namespace BuildingBlocks.EventStore.Repositories.Snapshots
{
    public class Snapshot<TAggregate> 
    {
        public TAggregate Aggregate { get; set; }
        public string AggregateId { get; set; } = string.Empty;
        public string AggregateType {  get; set; } = string.Empty; // ime konkretnog agregata
        public int Version { get; set; } // version agregata
        public DateTime CreatedAt { get; set; }
    }
}
