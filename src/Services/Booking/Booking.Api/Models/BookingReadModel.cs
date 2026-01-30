using Booking.Api.Dtos;
using Booking.Api.ValueObjects;
using MongoDB.Bson;

namespace Booking.Api.Models
{   
    // Svaki read model je kolekcija u Mongo
    public class BookingReadModel
    {   
        public ObjectId Id { get; init; } // Mongo ce sam da popuni, jer mu treba, a meni treba BookingId koju popunjavam iz koda
        public required Guid BookingId { get; init; }
        public required TripReadModelDto Trip { get; init; } // Mongo automatski serijalizuje custom types
        public required string PassengerInfo { get; init; }
        public required bool IsDeleted { get; init; }
    }
}
