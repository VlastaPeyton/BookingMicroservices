namespace BuildingBlocks.EventStore.Repositories.Snapshots
{
    public interface ISnapshotRepository<TAggregate, TId>
    {
        Task<Snapshot<TAggregate>?> GetSnapshotAsync(TId id, CancellationToken ct);
        Task SaveSnapshotAsync(TId id, TAggregate aggregate, int version, CancellationToken ct);
    }
}
