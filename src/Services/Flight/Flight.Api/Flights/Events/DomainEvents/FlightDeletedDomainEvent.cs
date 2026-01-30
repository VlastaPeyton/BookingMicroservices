using BuildingBlocks.Core.Events;
using Flight.Api.Flights.Enums;

namespace Flight.Api.Flights.Events.DomainEvents
{
    public record FlightDeletedDomainEvent(Guid FlightId,
                                          string FlightNumber,
                                          Guid AircraftId,
                                          DateTime DepartureDate,
                                          Guid DepartureAirportId,
                                          DateTime ArriveDate,
                                          Guid ArriveAirportId,
                                          decimal DurationMinutes,
                                          DateTime FlightDate,
                                          FlightStatusEnum Status,
                                          decimal Price,
                                          bool IsDeleted) : DomainEvent;
    
}
