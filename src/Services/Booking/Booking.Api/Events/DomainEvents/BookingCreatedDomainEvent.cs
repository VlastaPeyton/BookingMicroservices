using Booking.Api.ValueObjects;
using BuildingBlocks.Core.Events;

namespace Booking.Api.Events.DomainEvents
{
    public record BookingCreatedDomainEvent(Guid Id, 
                                            PassengerInfo PassengerInfo, 
                                            Trip Trip) : DomainEvent;

}
