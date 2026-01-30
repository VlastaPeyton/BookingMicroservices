

using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Events;

namespace BuildingBlocks.EventStore.Domain
{
    public abstract class AggregateRootEventSource<TId> : IAggregateRootEventSource<TId>
    {
        public TId Id { get; set; }

        public int Version { get; set; } = -1; // Automatski oznaci da je novi aggregaterooteventsource napravljen

        private readonly List<IDomainEvent> _uncommittedDomainEvents = new();
        public IReadOnlyList<IDomainEvent> DomainEvents => _uncommittedDomainEvents.AsReadOnly();

        // Za rehydratation kada citamo iz EventStoreDb
        public AggregateRootEventSource() { }

        public AggregateRootEventSource(TId id)
        {
            Id = id;
        }

        public IDomainEvent[] ClearDomainEvents()
        {
            var events = _uncommittedDomainEvents.ToArray();

            _uncommittedDomainEvents.Clear();

            return events;
        }

        // Ucitava sve evente za konkretni agregat i rekonstruise state
        public void LoadFromHistory(IEnumerable<IDomainEvent> domainEvents)
        {
            foreach (var domainEvent in domainEvents)
            {
                ((dynamic)this).When((dynamic)domainEvent); // Promeni state u memoriji konkretnog aggregata

                Version++;
            }
        }

        // Dodaj novi event u listu i promeni stanje agregata u memoriji
        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            ((dynamic)this).When((dynamic)domainEvent); // Promeni state u memoriji konkretnog aggregata
            _uncommittedDomainEvents.Add(domainEvent); // Dodaj u listu domainEvent
        }
    }
}
