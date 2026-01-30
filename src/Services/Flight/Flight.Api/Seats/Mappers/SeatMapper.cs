using Flight.Api.Seats.Dtos;
using Flight.Api.Seats.Enums;
using Flight.Api.Seats.Features.CreateSeat;
using Flight.Api.Seats.Features.GetAvailableSeats;
using Flight.Api.Seats.Features.ReserveSeat;

namespace Flight.Api.Seats.Mappers
{
    public static class SeatMapper
    {
        public static CreateSeatCommand FromCreateSeatRequestToCommand (this CreateSeatRequestDto request)
        {   // DDD bez EventSourcing, a ne zelim da Sql Db generise FlightId automatski, pa ga u kodu generisem ja u Command objektu
            return new CreateSeatCommand(Guid.NewGuid(), request.SeatNumber, request.SeatType, request.SeatClass, request.FlightId);
        }

        public static CreateSeatResponseDto FromCreateSeatResultToResponse(this CreateSeatResult result)
        {
            return new CreateSeatResponseDto(result.Id);
        }

        // C# u gRCP mapper
        public static SeatDtoResponse MapFromSeatDtoToGrpc(this SeatDto dto)
        {
            return new SeatDtoResponse
            {
                Id = dto.SeatId.ToString(),
                SeatNumber = dto.SeatNumber,
                Type = dto.Type.MapSeatTypeEnumToGrpc(), 
                Class = dto.Class.MapSeatClassEnumToGrpc(),
                FlightId = dto.FlightId.ToString(),
            };
        }
        
        private static SeatTypeGrpc MapSeatTypeEnumToGrpc(this SeatTypeEnum type)
        {
            return type switch
            {
                SeatTypeEnum.Window => SeatTypeGrpc.SeatTypeWindow,
                SeatTypeEnum.Middle => SeatTypeGrpc.SeatTypeMiddle,
                SeatTypeEnum.Aisle => SeatTypeGrpc.SeatTypeAisle,
                SeatTypeEnum.Unknown => SeatTypeGrpc.SeatTypeUnknown,
                _ => SeatTypeGrpc.SeatTypeUnknown,
            };
        }

        private static SeatClassGrpc MapSeatClassEnumToGrpc(this SeatClassEnum @class)
        {
            return @class switch
            {
                SeatClassEnum.FirstClass => SeatClassGrpc.SeatClassFirstClass,
                SeatClassEnum.Business => SeatClassGrpc.SeatClassBusiness,
                SeatClassEnum.Economy => SeatClassGrpc.SeatClassEconomy,
                SeatClassEnum.Unknown => SeatClassGrpc.SeatClassUnknown,
                _ => SeatClassGrpc.SeatClassUnknown,
            };
        }
    }
}
