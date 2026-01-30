using BuildingBlocks.Core.Events;

namespace BuildingBlocks.MassTransit.IntegrationEvents
{
    public record UserCreatedIntegrationEvent(Guid Id, 
                                              string PassengerName, 
                                              string PassportNumber, 
                                              int PassengerAge,
                                              string PassengerType // Ne sme PassengerTypeEnum, jer BB ne sme da referencira microservis u kom je enum definisan
                                              ) : IntegrationEvent;
}
