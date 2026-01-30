using System.Net;
using BuildingBlocks.Exceptions;

namespace Flight.Api.Seats.Exceptions
{
    public class SeatAlreadyExistException : AppException
    {
        public SeatAlreadyExistException(int? code = default) : base("Seat already exist!", HttpStatusCode.Conflict, code)
        {
        }
    }
}
