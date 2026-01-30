using BuildingBlocks.MassTransit.IntegrationEvents;
using MassTransit;
using MediatR;
using Passenger.Api.Enums;
using Passenger.Api.Features.CreatePassenger;


namespace Passenger.Api.EventHandlers.IntegrationEventHandlers
{
    public class PassengerCreatedIntegrationEventHandler : IConsumer<UserCreatedIntegrationEvent>
    {
        private readonly ISender _sender; // Pozivam CreatePassengerCommandHandler koji ce da upise u Sql i dispatch domain event, jer iz IntegrationEventHandler ne valja dispatch domain event
        
        public PassengerCreatedIntegrationEventHandler(ISender sender)
        {
            _sender = sender;
        }

        public async Task Consume(ConsumeContext<UserCreatedIntegrationEvent> context)
        {
            var passengerType = Enum.Parse<PassengerTypeEnum>(context.Message.PassengerType); // UserCreatedIntegrationEvent ne sme da zna za PassengerTypeEnum, vec string, pa mora cast

            var command = new CreatePassengerCommand(context.Message.Id,
                                                     context.Message.PassengerName,
                                                     context.Message.PassportNumber,
                                                     context.Message.PassengerAge,
                                                     passengerType);

            var result = await _sender.Send(command, context.CancellationToken);
        }
    }
}
