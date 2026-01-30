using BuildingBlocks.Core.Events;
using BuildingBlocks.EfCore;
using Identity.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Identity.Api.Data
{
    /* Ne mogu da nasledim i IdentityDbContext i ApplicationDbContextBase istovremeno, pa onda IApplicationDbContext da implementiram 
     iste metode kao u ApplicationDbContextBase samo bez audit i domain events dispatcher.
    */
    public class IdentityContextDb : IdentityDbContext<User, IdentityRole<Guid>, Guid>, IApplicationDbContext
    {   
        private IDbContextTransaction? _currentTransaction; // Pogledaj ApplicaitonDbContextBase

        public IdentityContextDb(DbContextOptions<IdentityContextDb> options) : base(options)
        {
            // Nema _currentTransaction, jer se ona kreira u runtime tokom BeginTransactionAsync
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
            throw new NotImplementedException(); 
            // Ne koristim ovu metodu ali mi treba da stoji jer sam interface koristio 
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
    }
}
