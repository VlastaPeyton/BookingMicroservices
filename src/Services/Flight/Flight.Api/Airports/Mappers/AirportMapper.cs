using Flight.Api.Airports.Features.CreateAirport;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Flight.Api.Airports.Mappers
{
    public static class AirportMapper
    {
        public static CreateAirportCommand FromCreateAirportRequestToCreateAirportCommand(this CreateAirportRequestDto dto)
        {   // DDD bez EventSourcing, a ne zelim da Sql Db generise AirportId automatski, pa ga u kodu generisem ja u Command objektu
            return new CreateAirportCommand(Guid.NewGuid(), dto.Name, dto.Address, dto.Code);
        }

        public static CreateAirportResponseDto FromCreateAirportResultToCreateAirportResponse(this CreateAirportResult result)
        {
            return new CreateAirportResponseDto(result.Id);
        }
    }
}
