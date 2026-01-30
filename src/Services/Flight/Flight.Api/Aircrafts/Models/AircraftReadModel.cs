using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Flight.Api.Aircrafts.Models
{
    // Read model mora imati samo primitive types, ne sme ValueObject, pa ako treba onda implicit operator u VO definsi zbog mapiranja u DomainEventHandler
    public class AircraftReadModel
    {
        [BsonId]
        public ObjectId Id { get; init; } // Mongo ce ovo da popuni jer mu je bitno, a meni je bitno AirportId koje popunim u kodu
        public required Guid AircraftId { get; init; }
        public required string Name { get; init; }
        public required string Model { get; init; }
        public required int ManufacturingYear { get; init; }
        public required bool IsDeleted { get; init; }

    }
}
