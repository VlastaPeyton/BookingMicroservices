using BuildingBlocks.Exceptions;

namespace Passenger.Api.Exception
{
    public class InvalidNameException : DomainException
    {
        public InvalidNameException() : base("Name cannot be empty or whitespace.")
        {
        }
    }
}
