using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Flight.Api.Airports.Models
{
    // Read model mora imati samo primitive types, ne sme ValueObject, pa ako treba onda implicit operator u VO definsi zbog mapiranja u DomainEventHandler
    public class AirportReadModel
    {
        [BsonId]   
        public ObjectId Id { get; init; } // Mongo ce ovo automatski, jer ovo je njemu bitno, a meni treba AirportId
        public required Guid AirportId { get; init; }
        public required string Name { get; init; }
        public string Address { get; init; }
        public required string Code { get; init; }
        public required bool IsDeleted { get; init; }
    }
}
