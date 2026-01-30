namespace BuildingBlocks.EventStore.Checkpoints
{   
    // Sluzi da upamtimo do kog event u stream sam stigo kad pravim projekciju
    public interface ICheckpointStore
    {
        Task<ulong?> GetCheckpointAsync(string checkpointName, CancellationToken ct);
        Task SaveCheckpointAsync(string checkpointName, ulong position, CancellationToken ct);
    }
}
