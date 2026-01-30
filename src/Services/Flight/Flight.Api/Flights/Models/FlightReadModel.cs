using Flight.Api.Flights.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Flight.Api.Flights.Models
{   
    // Read model mora imati samo primitive types, ne sme ValueObject, pa ako treba onda implicit operator u VO definsi zbog mapiranja u DomainEventHandler
    public class FlightReadModel
    {
        [BsonId]
        public ObjectId Id { get; init; } // Necu popuniti u kodu, ostavljam da Mongo to popuni, jer je ovo bitno za bazu, a meni treba FlightId
        public required Guid FlightId { get; init; }
        public required string FlightNumber { get; init; }
        public required Guid AircraftId { get; init; }
        public required DateTime DepartureDate { get; init; }
        public required Guid DepartureAirportId { get; init; }
        public required DateTime ArriveDate { get; init; }
        public required Guid ArriveAirportId { get; init; }
        public required decimal DurationMinutes { get; init; }
        public required DateTime FlightDate { get; init; }

        [BsonRepresentation(BsonType.String)] // Čuva enum kao string u Mongo i prevodi automatski u string kad treba
        public required FlightStatusEnum Status { get; init; } 
        public required decimal Price { get; init; }
        public required bool IsDeleted { get; init; }
    }
}
