using BuildingBlocks.Exceptions;

namespace Flight.Api.Seats.Exceptions
{
    public class SeatNumberIncorrectException : AppException
    {
        public SeatNumberIncorrectException() : base("Seat number is incorrect!")
        {
        }
    }
}
