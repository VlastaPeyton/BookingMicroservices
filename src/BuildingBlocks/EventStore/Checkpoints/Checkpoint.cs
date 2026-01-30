

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BuildingBlocks.EventStore.Checkpoints
{
    public class Checkpoint
    {
        [BsonId] // Mongo koristi ovo kao PK
        [BsonRepresentation(BsonType.String)] // Ako je Guid, Mongo ce da sacuva kao string
        public string CheckpointName { get; set; } = string.Empty; 
        // U SubscriptionToEventStore, _subcsriptionId + opciono streamName = CheckpointName 

        public ulong Position { get; set; }
        // = streamRevision ako SubscripitonToAllEventStore instanca pretplacena na samo 1 stream
        // globalna pozicija ako SubscripitonToAllEventStore instanca pretplacena na svaki stream 

        public DateTime UpdatedAt { get; set; }
    }
}
