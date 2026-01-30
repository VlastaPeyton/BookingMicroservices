

using System.Collections.Concurrent;
using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Interceptors
{   
    /* Enterprise dispatcher, ali ne koristim, jer ApplicationDbContextBase koristi UoW gde imam rucni commit, pa SaveChangesAsync nije commit automatski 
     cime ovo bi dispatchovalo events pre upisa u bazu, jer SaveChangesInterceptor automatski se pokrece na SaveChangesAsync bez obzira da l SaveChangesAsync 
     radi automatski commit ili ne. 
     Ovu logiku cu da prenesem u ApplicationDbContextBase.CommitTransactionAsync. 
     Ova logika vazi ako NEMAM EventSourcing kao npr u Flight microservice.
    */
    public class DispatchDomainEventsInterceptor : SaveChangesInterceptor
    {
        private readonly IMediator _mediator;

        // Enterprise-grade buffer za domain evente, thread-safe jer podrzava vise SaveChangesAsync u istom bloku 
        private readonly ConcurrentQueue<IDomainEvent> _domainEventsBuffer = new();
        public DispatchDomainEventsInterceptor(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Zbog enterprise grade 
        private void CollectDomainEventsBeforeSaveChanges(DbContext? context)
        {
            if (context is null) 
                return;

            var domainEvents = context.ChangeTracker.Entries<IAggregateRoot>()
                                                      .Where(e => e.Entity.DomainEvents.Any())
                                                      .SelectMany(e => e.Entity.DomainEvents)
                                                      .ToList();

            foreach (var aggregate in context.ChangeTracker.Entries<IAggregateRoot>())
                aggregate.Entity.ClearDomainEvents();

            foreach (var domainEvent in domainEvents)
                _domainEventsBuffer.Enqueue(domainEvent);
        }

        // Automatski se pokrece. Za svaki slucaj prikupi events pre nego baza commit da ih ne izgubim ako SavedChangesAsync ne vidi trackovane identite 
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
                                                                              InterceptionResult<int> result,
                                                                              CancellationToken cancellationToken = default)
        {
            CollectDomainEventsBeforeSaveChanges(eventData.Context);

            return ValueTask.FromResult(result);
        }


        // Automatski se pokrece. DispatchDomainEventsInterceptor zahteva SavedChangesAsync, jer se ona poziva nakon SaveChangesAsync jer dispatch domain events ide tek nakon uspesnog upisa u bazu
        public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            await DispatchDomainEventsAsync(eventData.Context, cancellationToken);

            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        private async Task DispatchDomainEventsAsync(DbContext? context, CancellationToken ct)
        {
            if (context is null)
                return;

            while (_domainEventsBuffer.TryDequeue(out var domainEvent))
            {
                await _mediator.Publish(domainEvent, ct); // Automatski aktivira klasu koja implementira INotificationHandler
            }

        }

        // Automatski se poziva ako commit pukne
        public override void SaveChangesFailed(DbContextErrorEventData eventData)
        {
            _domainEventsBuffer.Clear();
        }
    }
}
