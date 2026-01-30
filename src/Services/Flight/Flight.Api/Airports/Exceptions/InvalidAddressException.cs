using BuildingBlocks.Exceptions;

namespace Flight.Api.Airports.Exceptions
{
    public class InvalidAddressException : DomainException
    {
        public InvalidAddressException() : base("Address cannot be empty or whitespace.")
        {

        }
    }
    
}
