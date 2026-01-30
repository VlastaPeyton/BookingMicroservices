using Grpc.Core;
using MediatR;
using Passenger.Api.Features.GetPassengerById;
using Passenger.Api.Mappers;

namespace Passenger.Api.GrpcServer.Services
{
    public class PassengerGrpcEndpoints : PassengerGrpcService.PassengerGrpcServiceBase
    {
        private readonly ISender _sender;

        public PassengerGrpcEndpoints(ISender sender)
        {
            _sender = sender;
        }

        public override async Task<GetPassengerByIdResultGrpc> GetByIdGrpc(GetPassengerByIdRequestGrpc request, ServerCallContext context)
        {
            var result = await _sender.Send(new GetPassengerByIdQuery(new Guid(request.Id)), context.CancellationToken);

            // Mapiram iz GetPassengerByIdResult (C#) u GetPassengerByIdResultGrpc (passenger.proto) tj iz PassengerDto (C#) u PassengerResponse (.proto)
            var gRpcResponse = new GetPassengerByIdResultGrpc
            {
                // Zbog gRPC, moram navesti ime polja bez obzira na tip polja
                PassengerDto = new PassengerResponse
                {
                    Id = result.PassengerDto.PassengerId.ToString(),
                    Name = result.PassengerDto.PassengerName,
                    PassportNumber = result.PassengerDto.PassportNumber,
                    PassengerType = result.PassengerDto.PassengerType.MapPassengerTypeEnumToGrpc(), // U Mongo Status je tipa string, ali u result.FlightDto je FlightStatusEnum, ali u gRCP je FlightStatusGrpc
                    Age = result.PassengerDto.Age
                }
            };

            return gRpcResponse;
        }
    }
}