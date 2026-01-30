using Booking.Api.Dtos;
using Booking.Api.Features.CreateBooking;
using Booking.Api.ValueObjects;

namespace Booking.Api.Mapper
{
    public static class BookingMapper
    {
        public static CreateBookingCommand FromCreateBookingRequestToCreateBookingCommand(this CreateBookingRequestDto dto)
        {   // Rucno postavljam BookingId 
            return new CreateBookingCommand(Guid.NewGuid(), dto.PassengerId, dto.FlightId, dto.Description);
        }

        public static CreateBookingResponseDto FromCreateBookingResultToCreateBookingResponse(this CreateBookingResult result)
        {
            return new CreateBookingResponseDto(result.Id);
        }

        public static TripReadModelDto FromTripToTripReadModelDto(this Trip trip)
        {
            return new TripReadModelDto
            {
                FlightNumber = trip.FlightNumber,
                AircraftId = trip.AircraftId,
                DepartureAirportId = trip.DepartureAirportId,
                ArriveAirportId = trip.ArriveAirportId,
                FlightDate = trip.FlightDate,
                Price = trip.Price,
                Description = trip.Description,
                SeatNumber = trip.SeatNumber,
            };
        }
    }
}
