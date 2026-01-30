using BuildingBlocks.Exceptions;

namespace Booking.Api.Exceptions
{
    public class InvalidDepartureAirportIdException : DomainException
    {
        public InvalidDepartureAirportIdException(Guid departureAirportId) : base($"departureAirportId: '{departureAirportId}' is invalid.")
        {
        }
    }
}
