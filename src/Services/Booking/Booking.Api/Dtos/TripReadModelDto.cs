namespace Booking.Api.Dtos
{   
    // ReadModel ne sme imati ValueObject Trip, jer nije domain, vec Dto
    public class TripReadModelDto
    {
        public string FlightNumber { get; init; }
        public Guid AircraftId { get; init; }
        public Guid DepartureAirportId { get; init; }
        public Guid ArriveAirportId { get; init; }
        public DateTime FlightDate { get; init; }
        public decimal Price { get; init; }
        public string Description { get; init; }
        public int SeatNumber { get; init; }
    }
}
