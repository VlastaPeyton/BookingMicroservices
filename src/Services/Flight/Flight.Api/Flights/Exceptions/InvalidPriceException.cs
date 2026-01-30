using BuildingBlocks.Exceptions;

namespace Flight.Api.Flights.Exceptions
{
    public class InvalidPriceException : DomainException
    {
        public InvalidPriceException() : base($"Price Cannot be negative.")
        {
        }
    }
}
