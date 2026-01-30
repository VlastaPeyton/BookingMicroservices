using BuildingBlocks.Exceptions;

namespace Flight.Api.Aircrafts.Exceptions
{
    public class InvalidAircraftIdException : DomainException
    {
        public InvalidAircraftIdException(Guid aircraftId) : base($"AircraftId: '{aircraftId}' is invalid.")
        {
        }
    }
}
