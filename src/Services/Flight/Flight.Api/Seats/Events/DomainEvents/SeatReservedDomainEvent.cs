using BuildingBlocks.Core.Events;
using Flight.Api.Seats.Enums;
using Flight.Api.Seats.ValueObjects;

namespace Flight.Api.Seats.Events.DomainEvents
{
    public record SeatReservedDomainEvent(Guid SeatId, 
                                          int SeatNumber, 
                                          SeatTypeEnum Type, 
                                          SeatClassEnum Class,
                                          Guid FlightId, 
                                          bool IsDeleted) : DomainEvent;
   
}
