using System.Net;
using BuildingBlocks.Exceptions;
using Google.Rpc;

namespace Booking.Api.Exceptions
{
    public class BookingAlreadyExistsException : AppException
    {
        public BookingAlreadyExistsException() : base("Booking already exist!", HttpStatusCode.Conflict)
        {
            
        }
    }
}
