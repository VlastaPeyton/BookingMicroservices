using BuildingBlocks.Exceptions;

namespace Flight.Api.Flights.Exceptions
{
    public class InvalidArriveDateException : DomainException
    {
        public InvalidArriveDateException(DateTime arriveDate) : base($"Arrive Date: '{arriveDate}' is invalid.")
        {
        }
    }
}
