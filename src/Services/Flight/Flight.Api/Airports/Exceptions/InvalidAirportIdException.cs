using BuildingBlocks.Exceptions;

namespace Flight.Api.Airports.Exceptions
{
    public class InvalidAirportIdException : DomainException
    {
        public InvalidAirportIdException(Guid airportId) : base($"airportId: '{airportId}' is invalid.")
        {
        }
    }
}
