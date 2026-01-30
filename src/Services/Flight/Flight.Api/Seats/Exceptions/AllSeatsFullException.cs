using BuildingBlocks.Exceptions;

namespace Flight.Api.Seats.Exceptions
{
    public class AllSeatsFullException : AppException
    {
        public AllSeatsFullException() : base("All seats are full!")
        {
        }
    }
}
