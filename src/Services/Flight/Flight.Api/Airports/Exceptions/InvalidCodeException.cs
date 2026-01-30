using BuildingBlocks.Exceptions;

namespace Flight.Api.Airports.Exceptions
{
    public class InvalidCodeException : DomainException
    {
        public InvalidCodeException() : base("Code cannot be empty or whitespace.")
        {
        }
    }
}
