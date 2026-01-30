using BuildingBlocks.Core.Events;
using Passenger.Api.Enums;

namespace Passenger.Api.Events.DomainEvents
{
    public record PassengerRegistrationCompletedDomainEvent(Guid PassengerId, 
                                                           string Name, 
                                                           string PassportNumber,
                                                           PassengerTypeEnum PassengerType, 
                                                           int Age) : DomainEvent;
}
