using Flight.Api.Seats.Exceptions;

namespace Flight.Api.Seats.ValueObjects
{
    public record SeatId
    {
        public Guid Value { get; }

        private SeatId(Guid value)
        {
            Value = value;
        }

        public static SeatId Of(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new InvalidSeatIdException(value);
            }

            return new SeatId(value);
        }

        public static implicit operator Guid(SeatId seatId)
        {
            return seatId.Value;
        }
    }
}
