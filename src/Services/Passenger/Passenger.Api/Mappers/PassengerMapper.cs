using Passenger.Api.Enums;
using Passenger.Api.Features.CompleteRegistrationPassenger;

namespace Passenger.Api.Mappers
{
    public static class PassengerMapper
    {   
        public static CompleteRegistrationPassengerCommand FromCompleteRegistrationPassengerRequestToResult(this CompleteRegistrationPassengerRequestDto request)
        {   // DDD bez EventSourcing, a ne zelim da Sql Db generise FlightId automatski, pa ga u kodu generisem ja u Command objektu
            return new CompleteRegistrationPassengerCommand(Guid.NewGuid(), request.PassportNumber, request.PassengerType, request.Age);
        }

        public static CompleteRegistrationPassengerResponseDto FromCompleteRegistrationPassengerResultToResponse(this CompleteRegistrationPassengerResult result)
        {
            return new CompleteRegistrationPassengerResponseDto(result.PassengerDto);
        }

        public static PassengerTypeGrpc MapPassengerTypeEnumToGrpc(this PassengerTypeEnum passengerType)
        {
            return passengerType switch
            {
                PassengerTypeEnum.Baby => PassengerTypeGrpc.PassengerTypeBaby,
                PassengerTypeEnum.Female => PassengerTypeGrpc.PassengerTypeFemale,
                PassengerTypeEnum.Male => PassengerTypeGrpc.PassengerTypeMale,
                PassengerTypeEnum.Unknown => PassengerTypeGrpc.PassengerTypeUnknown,
                _ => PassengerTypeGrpc.PassengerTypeUnknown,
            };
        }
    }
}