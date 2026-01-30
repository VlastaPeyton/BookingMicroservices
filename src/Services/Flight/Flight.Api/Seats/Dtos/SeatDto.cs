using Flight.Api.Seats.Enums;

namespace Flight.Api.Seats.Dtos
{
    public record SeatDto(Guid SeatId, 
                          int SeatNumber, 
                          SeatTypeEnum Type, // U Mongo mi je lakse da sve string bude nego enum pa on da prevodi automatski iz tipa u tip
                          SeatClassEnum Class,  // U Mongo mi je lakse da sve string bude nego enum pa on da prevodi automatski iz tipa u tip
                          Guid FlightId);
}
