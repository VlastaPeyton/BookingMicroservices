using BuildingBlocks.Exceptions;

namespace Booking.Api.Exceptions
{
    public class InvalidFlightNumberException : DomainException
    {
        public InvalidFlightNumberException(string flightNumber) : base($"Flight Number: '{flightNumber}' is invalid.")
        {
        }
    }
}
