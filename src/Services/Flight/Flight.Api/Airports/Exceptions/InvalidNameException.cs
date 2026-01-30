using BuildingBlocks.Exceptions;

namespace Flight.Api.Airports.Exceptions
{
    public class InvalidNameException : DomainException
    {
        public InvalidNameException() : base("Name cannot be empty or whitespace.")
        {
        }
    }
}
