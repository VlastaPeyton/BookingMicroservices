using System.Net;
using BuildingBlocks.Exceptions;

namespace Passenger.Api.Exception
{
    public class PassengerNotFoundException : AppException
    {
        public PassengerNotFoundException() : base("Passenger not found!", HttpStatusCode.NotFound)
        {
        }
    }
}
