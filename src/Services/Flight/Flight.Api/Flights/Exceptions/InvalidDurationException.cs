using BuildingBlocks.Exceptions;

namespace Flight.Api.Flights.Exceptions
{
    public class InvalidDurationException : DomainException
    {
        public InvalidDurationException() : base("Duration cannot be negative.")
        {
        }
    }
}
