using BuildingBlocks.Exceptions;

namespace Flight.Api.Flights.Exceptions
{
    public class InvalidDepartureDateException : DomainException
    {
        public InvalidDepartureDateException(DateTime departureDate) : base($"Departure Date: '{departureDate}' is invalid.")
        {
        }
    }
}
