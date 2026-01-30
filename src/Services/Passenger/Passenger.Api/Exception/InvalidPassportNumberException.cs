using BuildingBlocks.Exceptions;

namespace Passenger.Api.Exception
{
    public class InvalidPassportNumberException : DomainException
    {
        public InvalidPassportNumberException() : base("Passport number cannot be empty or whitespace.")
        {
        }
    }
}
