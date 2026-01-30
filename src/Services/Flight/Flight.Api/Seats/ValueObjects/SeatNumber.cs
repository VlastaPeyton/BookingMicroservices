using Flight.Api.Seats.Exceptions;

namespace Flight.Api.Seats.ValueObjects
{
    public record SeatNumber
    {
        public int Value { get; }

        private SeatNumber(int value)
        {
            Value = value;
        }

        public static SeatNumber Of(int value)
        {
            if (value <0 || value > 500)
            {
                throw new InvalidSeatNumberException();
            }

            return new SeatNumber(value);
        }

        public static implicit operator int(SeatNumber seatNumber)
        {
            return seatNumber.Value;
        }
    }
}
