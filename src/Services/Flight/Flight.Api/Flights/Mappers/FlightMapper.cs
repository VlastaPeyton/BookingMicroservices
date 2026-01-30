using Flight.Api.Flights.Enums;
using Flight.Api.Flights.Features.CreateFlight;
using Flight.Api.Flights.Features.GetAvailableFlights;
using Flight.Api.Flights.Features.GetFlightById;
using Flight.Api.Flights.Features.UpdateFlight;

namespace Flight.Api.Flights.Mappers
{
    public static class FlightMapper
    {
        public static CreateFlightCommand FromCreateFlightRequestToCreateFlightCommand(this CreateFlightRequestDto request)
        {   // DDD bez EventSourcing, a ne zelim da Sql Db generise FlightId automatski, pa ga u kodu generisem ja u Command objektu
            return new CreateFlightCommand(Guid.NewGuid(),
                                           request.FlightNumber,
                                           request.AircraftId,
                                           request.DepartureDate,
                                           request.DepartureAirportId,
                                           request.ArriveDate,
                                           request.ArriveAirportId,
                                           request.DurationMinutes,
                                           request.FlightDate,
                                           request.Status,
                                           request.Price);

        }

        public static CreateFlightResponseDto FromCreateFlightResultToCreateFlightResponse(this CreateFlightResult result)
        {
            return new CreateFlightResponseDto(result.Id);
        }

        public static UpdateFlightCommand FromUpdateFlightRequestToUpdateFlightCommand(this UpdateFlightRequestDto request)
        {
            return new UpdateFlightCommand(request.FlightId, 
                                           request.FlightNumber,
                                           request.AircraftId,
                                           request.DepartureDate,
                                           request.DepartureAirportId,
                                           request.ArriveDate,
                                           request.ArriveAirportId,
                                           request.DurationMinutes,
                                           request.FlightDate,
                                           request.Status,
                                           request.Price,
                                           request.IsDeleted);
        }

        public static GetAvailableFlightsResponseDto FromGetAvailableFlightsResultToResponse(this GetAvailableFlightsResult result)
        {
            return new GetAvailableFlightsResponseDto(result.FlightDtos);
        }

        public static GetFlightByIdResponseDto FromGetFlightByIdRequestToResponse(this Flight.Api.Flights.Features.GetFlightById.GetFlightByIdResult result)
        {
            return new GetFlightByIdResponseDto(result.FlightDto);
        }

        // C# u gRCP mapiranje
        public static FlightStatusGrpc MapFlightStatusEnumToGrpc(this FlightStatusEnum status)
        {
            return status switch
            {
                FlightStatusEnum.Unknown => FlightStatusGrpc.FlightStatusUnknown,
                FlightStatusEnum.Flying => FlightStatusGrpc.FlightStatusFlying,
                FlightStatusEnum.Delay => FlightStatusGrpc.FlightStatusDelay,
                FlightStatusEnum.Canceled => FlightStatusGrpc.FlightStatusCanceled,
                FlightStatusEnum.Completed => FlightStatusGrpc.FlightStatusCompleted,
                _ => FlightStatusGrpc.FlightStatusUnknown,
            };
        }

    }
}
