

using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Events;

namespace BuildingBlocks.EventStore.Domain
{
    public interface IAggregateRootEventSource<TId>
    {
        TId Id { get; }
        int Version { get; } // Pozicija eventa u stream
        IReadOnlyList<IDomainEvent> DomainEvents { get; }
        IDomainEvent[] ClearDomainEvents();
    }
}
