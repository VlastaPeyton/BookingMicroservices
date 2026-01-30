using Flight.Api.Seats.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Flight.Api.Seats.Models
{
    public class SeatReadModel
    {
        [BsonId]
        public ObjectId Id { get; init; } // Necu popuniti u kodu, ostavljam da Mongo to popuni, jer je ovo bitno za bazu, a meni treba FlightId
        public required Guid SeatId { get; init; }
        public required int SeatNumber { get; init; }

        [BsonRepresentation(BsonType.String)] // Čuva enum kao string u Mongo i prevodi automatski u string kad treba
        public required SeatTypeEnum Type { get; init; }
        [BsonRepresentation(BsonType.String)] // Čuva enum kao string u Mongo i prevodi automatski u string kad treba
        public required SeatClassEnum Class { get; init; }
        public required Guid FlightId { get; init; }
        public required bool IsDeleted { get; init; }
    }
}
