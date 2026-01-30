using Flight.Api.Aircrafts.Features.CreateAircraft;

namespace Flight.Api.Aircrafts.Mappers
{   
    public static class AircraftMapper
    {   
        public static CreateAircraftCommand FromCreateAircraftRequestToCreateAircraftCommand(this CreateAircraftRequestDto dto)
        {   // DDD bez EventSourcing, a ne zelim da Sql Db generise aircraftId automatski, pa ga u kodu generisem ja
            return new CreateAircraftCommand(Guid.NewGuid(), dto.Name, dto.Model, dto.ManufacturingYear);
        }

        public static CreateAircraftResponseDto FromCreateAircraftResultToCreateAircraftResponse(this CreateAircraftResult result)
        {
            return new CreateAircraftResponseDto(result.Id);
        }
    }
}
