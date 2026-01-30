using BuildingBlocks.Core.Events;

namespace Flight.Api.Aircrafts.Events.DomainEvents
{
    public record AircraftCreatedDomainEvent(Guid Id, 
                                            string Name, 
                                            string Model, 
                                            int ManufacturingYear) : DomainEvent;
    
}
