using BuildingBlocks.Core.Domain;
using Flight.Api.Flights.ValueObjects;
using Flight.Api.Seats.Enums;
using Flight.Api.Seats.Events.DomainEvents;
using Flight.Api.Seats.ValueObjects;

namespace Flight.Api.Seats.Models
{
    // Ovo nije EventSourcing, vec obican DDD
    public class Seat : AggregateRoot<SeatId>
    {
        public SeatNumber SeatNumber { get; private set; } = default!;
        public SeatTypeEnum Type { get; private set; }
        public SeatClassEnum Class { get; private set; }
        public FlightId FlightId { get; private set; } = default!; // FK

        public static Seat Create(SeatId seatId, 
                                  SeatNumber seatNumber, 
                                  SeatTypeEnum seatType, 
                                  SeatClassEnum seatClass,
                                  FlightId flightId)
        {
            var seat = new Seat
            {
                Id = seatId,
                SeatNumber = seatNumber,
                Type = seatType,
                Class = seatClass,
                FlightId = flightId,
            };

            var domainEvent = new SeatCreatedDomainEvent(seat.Id,
                                                         seat.SeatNumber,
                                                         seat.Type,
                                                         seat.Class,
                                                         seat.FlightId);
                
            seat.AddDomainEvent(domainEvent);

            return seat;
        }

        public void ReserveSeat()
        {
            var isDeleted = true; 

            var domainEvent = new SeatReservedDomainEvent(Id,
                                                          SeatNumber,
                                                          Type,
                                                          Class,
                                                          FlightId,
                                                          isDeleted); 

            AddDomainEvent(domainEvent);
        }
    }
}
