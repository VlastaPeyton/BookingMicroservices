

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BuildingBlocks.EventStore.Subscriptions
{   
    // Event koji je vec obradjen u EventStore da spreci duplo procesiranje istog eventa, retry ako Status=failed itd
    public class ProcessedEvent
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)] // Objasnjeno u Checkpoint
        public string EventId { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty; // tip eventa 
        public DateTime ProcessedAt { get; set; }
        public string? Error { get; set; } // Ako se sjebe nesto pri upisu
        public string Status { get; set; } = string.Empty;
    }
}
