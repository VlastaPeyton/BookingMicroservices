using BuildingBlocks.Exceptions;

namespace Booking.Api.Exceptions
{
    public class InvalidPassengerNameException : DomainException
    {
        public InvalidPassengerNameException(string passengerName) : base($"Passenger Name: '{passengerName}' is invalid.")
        {
        }
    }
}
