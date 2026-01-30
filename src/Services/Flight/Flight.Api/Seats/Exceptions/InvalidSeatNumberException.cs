using BuildingBlocks.Exceptions;

namespace Flight.Api.Seats.Exceptions
{
    public class InvalidSeatNumberException : DomainException
    {
        public InvalidSeatNumberException() : base("SeatNumber Cannot be null or negative")
        {
        }
    }
}
