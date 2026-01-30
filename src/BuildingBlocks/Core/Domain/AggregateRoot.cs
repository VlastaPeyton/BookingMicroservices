

using BuildingBlocks.Core.Events;

namespace BuildingBlocks.Core.Domain
{   
    // Audit u Sql Server
    public abstract class AggregateRoot<TId>: Entity<TId>, IAggregateRoot<TId>
    {
        private readonly List<IDomainEvent> _domainEvents = new();  
        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public IDomainEvent[] ClearDomainEvents()
        {
            IDomainEvent[] dequedEvents = _domainEvents.ToArray();

            _domainEvents.Clear(); 

            return dequedEvents;
        }

        // Ova metoda ne vazi za IAggregateRoot jer interface nema protected metode
        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }
    }
}
