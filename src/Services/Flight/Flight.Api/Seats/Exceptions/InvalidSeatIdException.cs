using BuildingBlocks.Exceptions;

namespace Flight.Api.Seats.Exceptions
{
    public class InvalidSeatIdException : DomainException
    {
        public InvalidSeatIdException(Guid seatId) : base($"seatId: '{seatId}' is invalid.")

        {
        }
    }
}
