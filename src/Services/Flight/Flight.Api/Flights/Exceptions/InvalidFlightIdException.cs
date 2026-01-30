using BuildingBlocks.Exceptions;

namespace Flight.Api.Flights.Exceptions
{
    public class InvalidFlightIdException : DomainException
    {
        public InvalidFlightIdException(Guid flightId) : base($"flightId: '{flightId}' is invalid.")
        {
        }
    }
}
