using System.Net;
using BuildingBlocks.Exceptions;

namespace Passenger.Api.Exception
{
    public class PassengerNotExistException : AppException
    {
        public PassengerNotExistException() : base("Please register before!", HttpStatusCode.NotFound)
        {
        }
    }
}
