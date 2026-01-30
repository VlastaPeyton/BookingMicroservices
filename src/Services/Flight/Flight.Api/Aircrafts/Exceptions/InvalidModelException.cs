using BuildingBlocks.Exceptions;

namespace Flight.Api.Aircrafts.Exceptions
{
    public class InvalidModelException : DomainException
    {
        public InvalidModelException() : base("Model cannot be empty or whitespace.")
        {
        }
    }
}
