using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Passenger.Api.Enums;

namespace Passenger.Api.Models
{   
    // Read model samo primitive types da ima
    public class PassengerReadModel
    {
        [BsonId]
        public ObjectId Id { get; init; } // Necu popuniti u kodu, ostavljam da Mongo to popuni, jer je ovo bitno za bazu, a meni treba FlightId
        public required Guid PassengerId { get; init; }
        public required string PassportNumber { get; init; }
        public required string Name { get; init; }

        [BsonRepresentation(BsonType.String)] // Čuva enum kao string u Mongo i prevodi automatski u string kad treba
        public required PassengerTypeEnum PassengerType { get; init; }
        public int Age { get; init; }
        public required bool IsDeleted { get; init; }
    }
}
