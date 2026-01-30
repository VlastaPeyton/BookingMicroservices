using BuildingBlocks.Core.Events;
using Passenger.Api.Enums;

namespace Passenger.Api.Events.DomainEvents
{
    public record PassengerCreatedDomainEvent(Guid PassengerId,
                                              string PassengerName,
                                              string PassportNumber,
                                              int Age,
                                              PassengerTypeEnum PassengerType) : DomainEvent;


}
