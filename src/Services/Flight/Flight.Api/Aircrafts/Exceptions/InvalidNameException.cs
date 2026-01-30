using BuildingBlocks.Exceptions;

namespace Flight.Api.Aircrafts.Exceptions
{
    public class InvalidNameException : DomainException
    {
        public InvalidNameException() : base("Name cannot be empty or whitespace.")
        {
        }
    }
}
