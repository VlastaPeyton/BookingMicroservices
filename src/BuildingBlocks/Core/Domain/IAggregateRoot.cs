

using BuildingBlocks.Core.Events;

namespace BuildingBlocks.Core.Domain
{
    public interface IAggregateRoot : IEntity
    {
        public IReadOnlyList<IDomainEvent> DomainEvents {get;}
        public IDomainEvent[] ClearDomainEvents();
    }

    public interface IAggregateRoot<TId> : IEntity<TId>, IAggregateRoot { }
}
