using BuildingBlocks.EventStore;

namespace Booking.Api.ValueObjects
{   
    // ValueObject Id za event sourcing mora da nasledi IPersistableId
    public record BookingId : IPersistableId 
    {   
        public Guid Value { get; }
        private BookingId(Guid value)
        {
            Value = value;
        }
        public static BookingId Of(Guid value)
        {
            return new BookingId(value);
        }
        public string PersistToString()
        {
            return Value.ToString();
        }

        // Implicitna konverzija iz Guid zbog AggregateRootEventSource
        public static implicit operator BookingId(Guid value) => new BookingId(value);

        // Implicitna konverzija u Guid zbog AggregateRootEventSource
        public static implicit operator Guid(BookingId id) => id.Value;
    }
}
