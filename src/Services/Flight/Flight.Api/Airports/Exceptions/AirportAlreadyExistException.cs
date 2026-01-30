using System.Net;
using BuildingBlocks.Exceptions;

namespace Flight.Api.Airports.Exceptions
{
    public class AirportAlreadyExistException : AppException
    {
        public AirportAlreadyExistException(int? code = default) : base("Airport already exist!", HttpStatusCode.Conflict, code)
        {

        }
    }
}
