using BuildingBlocks.EventStore.Domain;

namespace BuildingBlocks.EventStore.Repositories.EvetnStore
{
    public interface IEventStoreRepository<TAggregate, TId>
        where TAggregate : AggregateRootEventSource<TId>
    {
        // Ucitava agregat iz EventStoreDb primenom svih events
        Task<TAggregate?> GetStreamByIdAsync(TId id, CancellationToken ct);

        // Upise uncommitedEvents iz agregata u EventStoreDb
        Task SaveUncommittedEventsAsync(TAggregate aggregate, 
                                        string? userId, 
                                        string? correlationId,
                                        CancellationToken ct);

        // Proverava da li stream postoji u EventStoreDb
        Task<bool> StreamExistAsync(TId  id, CancellationToken ct);

        // Soft delete stream 
        Task DeleteStreamAsync(TId id, CancellationToken ct);
    }
}
