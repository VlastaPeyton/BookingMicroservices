using Booking.Api.Exceptions;

namespace Booking.Api.ValueObjects
{
    public record PassengerInfo
    {
        public string Name { get; }

        private PassengerInfo(string name)
        {
            Name = name;
        }

        public static PassengerInfo Of(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidPassengerNameException(name);
            }

            return new PassengerInfo(name);
        }

        public static implicit operator PassengerInfo(string name) => Of(name);

        public static implicit operator string(PassengerInfo passengerInfo) => passengerInfo.Name;
    }
}
