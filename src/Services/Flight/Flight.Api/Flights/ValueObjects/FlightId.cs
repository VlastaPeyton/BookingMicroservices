using BuildingBlocks.EventStore;

namespace Flight.Api.Flights.ValueObjects
{   
    // Ne nasledjuje IPersistableId jer koristim AggregateRoot a ne AggregateRootEventSource
    public class FlightId
    {
        public Guid Value { get; }
        private FlightId(Guid value)
        {
            Value = value;
        }
        public static FlightId Of(Guid value)
        {
            return new FlightId(value);
        }

        // Implicitna konverzija iz Guid zbog AggregateRootEventSource
        public static implicit operator FlightId(Guid value) => new FlightId(value);

        // Implicitna konverzija u Guid zbog AggregateRootEventSource
        public static implicit operator Guid(FlightId id) => id.Value;
    }
}
