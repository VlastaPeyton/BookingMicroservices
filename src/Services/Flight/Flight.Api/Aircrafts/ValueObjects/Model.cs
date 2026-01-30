using Flight.Api.Aircrafts.Exceptions;

namespace Flight.Api.Aircrafts.ValueObjects
{
    public record Model
    {
        public string Value { get; }

        private Model(string value)
        {
            Value = value;
        }

        public static Model Of(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidModelException();
            }

            return new Model(value);
        }

        public static implicit operator string(Model model)
        {
            return model.Value;
        }
    }
}
