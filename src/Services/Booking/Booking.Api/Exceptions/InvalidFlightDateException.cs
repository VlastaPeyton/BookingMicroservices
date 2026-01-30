using BuildingBlocks.Exceptions;

namespace Booking.Api.Exceptions
{
    public class InvalidFlightDateException : DomainException
    {
        public InvalidFlightDateException(DateTime flightDate) : base($"Flight Date: '{flightDate}' is invalid.")
        {
        }
    }
}
