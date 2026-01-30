using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Events;
using BuildingBlocks.Interceptors;
using BuildingBlocks.UserProviders;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace BuildingBlocks.EfCore
{   
    // Necu da registrujem ovu klasu u DI, vec konkrentu implementaciju
    public class ApplicationDbContextBase : DbContext, IApplicationDbContext
    {
        private IDbContextTransaction? _currentTransaction;
        private readonly ICurrentUserProvider? _currentUserProvider;
        private readonly IMediator _mediator;
        public ApplicationDbContextBase(DbContextOptions options, 
                                        ICurrentUserProvider? currentUserProvider,
                                        IMediator mediator) : base(options)
        {
            // Nema _currentTransaction, jer se ona kreira u runtime tokom BeginTransactionAsync
            _currentUserProvider = currentUserProvider;
            _mediator = mediator;
            // Zbog postojanja barem 1 DI u ctor, potrebna je DesignTimeDbContextFactoryBase.cs zbog migracije samo - pogledaj DesignTimeDbContextFactory.txt
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Override cu u svakom microservicu jer je ovo custom 

            // Svaki microservis bice Publisher + Consumer na MessageBroker(MassTransit)
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
            modelBuilder.AddInboxStateEntity();
        }
        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {   
            // Zastita, jer ne sme vise od 1 transakcije nad istim DbContext istovremeno
            if (_currentTransaction != null)
                return; 

            _currentTransaction = await Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {   // Zbog ovoga, SaveChangesAsync nece imati automatski commit i zato DispatchDomainEventsInterceptor ne koristim 
            try
            {
                await SaveChangesAsync(cancellationToken);

                if (_currentTransaction is not null)
                    await _currentTransaction?.CommitAsync(cancellationToken)!;

                // Dispatch eventa tek posle commit u bazu
                var domainEvents = GetAndClearDomainEvents();
                foreach (var domainEvent in domainEvents)
                {
                    await _mediator.Publish(domainEvent, cancellationToken);
                }
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
        {
            if (_currentTransaction is null)
                return; 

            try
            {
                await _currentTransaction?.RollbackAsync(cancellationToken)!;
            }
            finally
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null;
            }
        }

        // Definisano u Program.cs ili Extension kakvi su parametri retry za transakciju koja nije uspela 
        public IExecutionStrategy CreateExecutionStrategy() => Database.CreateExecutionStrategy(); 

        public Task ExecuteTransactionalAsync(CancellationToken cancellationToken)
        {
            var strategy = CreateExecutionStrategy(); // retry za transient greske ako pukne konekcija, db bila zauzeta itd.

            // Svaki retry otvara novu transakciju
            return strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    await SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                    // Nakon commit koji je sada uspesan, dispatch domain events
                    var domainEvents = GetAndClearDomainEvents();
                    foreach (var domainEvent in domainEvents)
                    {
                        await _mediator.Publish(domainEvent, cancellationToken);
                    }
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });
        }

        public IReadOnlyList<IDomainEvent> GetAndClearDomainEvents()
        {
            var aggregates = ChangeTracker.Entries<IAggregateRoot>()
                                          .Where(x => x.Entity.DomainEvents.Any())
                                          .Select(x => x.Entity);
                                            
            var domainEvents = aggregates.SelectMany(x => x.DomainEvents)
                                         .ToList();

            aggregates.ToList().ForEach(a => a.ClearDomainEvents());

            return domainEvents;
        }

        // Concurrency problem handling
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            // SaveChangesAsync by default vrati broj (int) promena u bazi

            // Pozovi BeforeSaveChanges samo ako AuditInterceptor nije registrovan 
            if (!HasAuditInterceptor())
            {
                BeforeSaveChanges();
            }

            try 
            {   // Klasika SaveChangesAsync
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {   
                // Entiteti koji su izazvali konflikt u bazi
                foreach (var entry in ex.Entries)
                {   
                    // Dohvati trenutnu vrednost iz baze za konfliktni entitet 
                    var databaseValues = await entry.GetDatabaseValuesAsync(cancellationToken);
                    if (databaseValues is null)
                        throw; // Ako je neko vec izbrisao vrstu iz baze pomocu soft/hard delete, bacam exception

                    // Postavi trenutne vrednosti iz baze cime prejebem concurrency check
                    entry.OriginalValues.SetValues(databaseValues);
                }
                return await base.SaveChangesAsync(cancellationToken);
            }
        }

        // Provera da li je AuditInterceptor registrovan
        private bool HasAuditInterceptor()
        {
            try
            {
                var auditInterceptor = Database.GetService<AuditInterceptor>(); 
                // Database jer sam u registrovao AuditInterceptor nad ApplicationDbContext, pa to ode u EF Core DI takodje
                if (auditInterceptor is null)
                    return false;
                else
                    return true;
            }
            catch
            {
                // Ako ne može da proveri, pretpostavi da nema interceptor
                return false;
            }
        }

        // Audit interceptor za DbContext 
        private void BeforeSaveChanges()
        {   
            // Ovo je ujedno i AuditInterceptor 
            var now = DateTime.UtcNow; // Ako petlja ima 100entiteta, osigurava da svi imaju isto vreme
            string? userName = _currentUserProvider?.GetCurrentUserId();
            try
            {
                foreach (var entry in ChangeTracker.Entries<IEntity>()) 
                {   
                    if (entry.State == EntityState.Added) // Kada koristim DbContext.Add
                    {
                        entry.Entity.CreatedBy = userName;
                        entry.Entity.CreatedAt = now;
                    }

                    else if (entry.State == EntityState.Modified) // Kada koristim DbContext.Update
                    {   
                        // Ne salji update u bazu za ova polja 
                        entry.Property(x => x.CreatedAt).IsModified = false;
                        entry.Property(x => x.CreatedBy).IsModified = false;

                        entry.Entity.LastModifiedBy = userName;
                        entry.Entity.LastModifiedAt = now;
                    }

                    else if (entry.State == EntityState.Deleted) // Kada koristim DbContext.Remove
                    {   
                        // Soft delete ne sme imati EntityState.Deleted jer ce izbrisati iz baze ceo red 
                        entry.State = EntityState.Modified;

                        // Ne salji update u bazu za ova polja 
                        entry.Property(x => x.CreatedAt).IsModified = false;
                        entry.Property(x => x.CreatedBy).IsModified = false;

                        entry.Entity.LastModifiedBy = userName;
                        entry.Entity.LastModifiedAt = now;
                        entry.Entity.IsDeleted = true; // soft delete
                    }    
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
