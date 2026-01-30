using Flight.Api.Flights.Exceptions;

namespace Flight.Api.Flights.ValueObjects
{
    public record DepartureDate
    {
        public DateTime Value { get; }

        private DepartureDate(DateTime value)
        {
            Value = value;
        }

        public static DepartureDate Of(DateTime value)
        {
            if (value == default)
            {
                throw new InvalidDepartureDateException(value);
            }

            return new DepartureDate(value);
        }

        // Zbog AggregateRootEventSource
        public static implicit operator DateTime(DepartureDate departureDate)
        {
            return departureDate.Value;
        }
    }
}
