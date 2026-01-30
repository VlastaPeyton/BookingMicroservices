using System.Net;
using BuildingBlocks.Exceptions;

namespace Flight.Api.Aircrafts.Exceptions
{
    public class AircraftAlreadyExistException : AppException
    {
        public AircraftAlreadyExistException() : base("Aircraft already exist!", HttpStatusCode.Conflict)
        {
        }
    }
}
