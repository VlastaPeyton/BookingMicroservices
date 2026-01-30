using System.Net;
using BuildingBlocks.Exceptions;

namespace Booking.Api.Exceptions
{
    public class InvalidAircraftIdException : DomainException
    {
        public InvalidAircraftIdException(Guid aircraftId) : base($"aircraftId: '{aircraftId}' is invalid.")
        {
        }
    }
}
