

using BuildingBlocks.Core.Events;

namespace BuildingBlocks.EventStore.Subscriptions
{   
    /* Ne moze INotificationHandler iz MediatR, jer on sluzi za in-process, a ovo je persistance jer se koristi za EventSourcing 
     tj Subscriber na EventStore poziva DomainEventHandlerES za AggregateRootEventSourcing
       Ovo ne moze za AggregateRoot, jer se tamo koristi MediatR za DomainEventHandler.*/
    public interface IDomainEventHandlerES
    {
        Task HandleAsync(IDomainEvent domainEvent, CancellationToken ct);
        bool CanHandle(Type eventType);
    }
}
