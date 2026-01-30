using System.Net;
using BuildingBlocks.Exceptions;

namespace Booking.Api.Exceptions
{
    public class FlightNotFoundException : AppException
    {
        public FlightNotFoundException() : base("Flight doesnt exists", HttpStatusCode.NotFound)
        {
        }
    }
}
