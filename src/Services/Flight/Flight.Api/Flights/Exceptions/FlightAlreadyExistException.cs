using System.Net;
using BuildingBlocks.Exceptions;

namespace Flight.Api.Flights.Exceptions
{
    public class FlightAlreadyExistException : AppException
    {
        public FlightAlreadyExistException(int? code = default) : base("Flight already exist!", HttpStatusCode.Conflict, code)
        {

        }
    }
}
