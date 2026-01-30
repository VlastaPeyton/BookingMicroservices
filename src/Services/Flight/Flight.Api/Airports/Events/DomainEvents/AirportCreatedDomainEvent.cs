using BuildingBlocks.Core.Events;

namespace Flight.Api.Airports.Events.DomainEvents
{
    public record AirportCreatedDomainEvent(Guid Id, string Name, string Address, string Code) : DomainEvent;
}
