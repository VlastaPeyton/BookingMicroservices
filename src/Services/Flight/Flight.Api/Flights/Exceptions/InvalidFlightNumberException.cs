using BuildingBlocks.Exceptions;

namespace Flight.Api.Flights.Exceptions
{
    public class InvalidFlightNumberException : DomainException
    {
        public InvalidFlightNumberException(string flightNumber) : base($"Flight Number: '{flightNumber}' is invalid.")
        {
        }
    }
}
