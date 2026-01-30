using Passenger.Api.Exception;

namespace Passenger.Api.ValueObjects
{
    public record Age
    {
        public int Value { get; }

        private Age(int value)
        {
            Value = value;
        }

        public static Age Of(int value)
        {
            if (value <= 0)
            {
                throw new InvalidAgeException();
            }

            return new Age(value);
        }

        public static implicit operator int(Age age)
        {
            return age.Value;
        }
    }
}
