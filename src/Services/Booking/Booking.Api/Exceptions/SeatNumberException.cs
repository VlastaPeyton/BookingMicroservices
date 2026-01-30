using BuildingBlocks.Exceptions;

namespace Booking.Api.Exceptions
{
    public class SeatNumberException : DomainException
    {
        public SeatNumberException(int seatNumber) : base($"Seat Number: '{seatNumber}' is invalid.")
        {
        }
    }
}
