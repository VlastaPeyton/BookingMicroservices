using BuildingBlocks.Exceptions;

namespace Booking.Api.Exceptions
{
    public class InvalidArriveAirportIdException : DomainException
    {
        public InvalidArriveAirportIdException(Guid arriveAirportId) : base($"arriveAirportId: '{arriveAirportId}' is invalid.")
        {
        }
    }
}
