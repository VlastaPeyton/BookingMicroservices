using Flight.Api.Flights.Enums;

namespace Flight.Api.Flights.Dtos
{
    public record FlightDto(Guid FlightId, 
                            string FlightNumber, 
                            Guid AircraftId,
                            DateTime ArriveDate,
                            Guid ArriveAirportId,
                            DateTime DepartureDate,
                            Guid DepartureAirportId,
                            decimal DurationMinutes, 
                            DateTime FlightDate,
                            FlightStatusEnum Status, // U Mongo mi je lakse da sve string bude nego enum pa on da prevodi automatski iz tipa u tip
                            decimal Price);
}
