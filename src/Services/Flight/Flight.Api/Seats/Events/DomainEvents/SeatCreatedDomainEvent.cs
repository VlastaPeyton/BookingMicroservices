using BuildingBlocks.Core.Events;
using Flight.Api.Seats.Enums;

namespace Flight.Api.Seats.Events.DomainEvents
{
    public record SeatCreatedDomainEvent(Guid SeatId, 
                                        int SeatNumber, 
                                        SeatTypeEnum Type, 
                                        SeatClassEnum Class,
                                        Guid FlightId) : DomainEvent;
}
