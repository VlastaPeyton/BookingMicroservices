using BuildingBlocks.Exceptions;

namespace Passenger.Api.Exception
{
    public class InvalidAgeException : DomainException
    {
        public InvalidAgeException() : base("Age Cannot be null or negative")
        {
        }

    }
}
