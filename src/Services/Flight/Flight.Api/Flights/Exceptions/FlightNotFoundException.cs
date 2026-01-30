using System.Net;
using BuildingBlocks.Exceptions;

namespace Flight.Api.Flights.Exceptions
{
    public class FlightNotFountException : AppException
    {
        public FlightNotFountException() : base("Flight not found!", HttpStatusCode.NotFound)
        {
        }
    }
}
