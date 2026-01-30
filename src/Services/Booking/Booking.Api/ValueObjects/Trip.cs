using Booking.Api.Exceptions;

namespace Booking.Api.ValueObjects
{
    public record Trip
    {
        public string FlightNumber { get; }
        public Guid AircraftId { get; }
        public Guid DepartureAirportId { get; }
        public Guid ArriveAirportId { get; }
        public DateTime FlightDate { get; }
        public decimal Price { get; }
        public string Description { get; }
        public int SeatNumber { get; }

        private Trip(string flightNumber, 
                    Guid aircraftId, 
                    Guid departureAirportId, 
                    Guid arriveAirportId,
                    DateTime flightDate, 
                    decimal price, 
                    string description, 
                    int seatNumber)
        {
            FlightNumber = flightNumber;
            AircraftId = aircraftId;
            DepartureAirportId = departureAirportId;
            ArriveAirportId = arriveAirportId;
            FlightDate = flightDate;
            Price = price;
            Description = description;
            SeatNumber = seatNumber;
        }

        public static Trip Of(string flightNumber, 
                              Guid aircraftId, 
                              Guid departureAirportId, 
                              Guid arriveAirportId,
                              DateTime flightDate, 
                              decimal price, 
                              string description, 
                              int seatNumber)
        {   
            if (string.IsNullOrWhiteSpace(flightNumber))
            {
                throw new InvalidFlightNumberException(flightNumber);
            }

            if (aircraftId == Guid.Empty)
            {
                throw new InvalidAircraftIdException(aircraftId);
            }

            if (departureAirportId == Guid.Empty)
            {
                throw new InvalidDepartureAirportIdException(departureAirportId);
            }

            if (arriveAirportId == Guid.Empty)
            {
                throw new InvalidArriveAirportIdException(departureAirportId);
            }

            if (flightDate == default)
            {
                throw new InvalidFlightDateException(flightDate);
            }

            if (price < 0)
            {
                throw new InvalidPriceException(price);
            }

            if (seatNumber < 1 || seatNumber > 220)
            {
                throw new SeatNumberException(seatNumber);
            }

            return new Trip(flightNumber, aircraftId, departureAirportId, arriveAirportId, flightDate, price, description, seatNumber);
        }
    }
}
