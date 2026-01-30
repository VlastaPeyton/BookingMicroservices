using Carter;
using Flight.Api.Flights.Enums;
using Flight.Api.Flights.Mappers;
using MediatR;

namespace Flight.Api.Flights.Features.CreateFlight
{   
    public record CreateFlightRequestDto(string FlightNumber, 
                                         Guid AircraftId,
                                         DateTime DepartureDate,
                                         Guid DepartureAirportId,
                                         DateTime ArriveDate,
                                         Guid ArriveAirportId,
                                         decimal DurationMinutes, 
                                         DateTime FlightDate,
                                         FlightStatusEnum Status, // U BB registrovao da string iz Http Request mapira u Enum iz RequestDto 
                                         decimal Price);

    public record CreateFlightResponseDto(Guid Id);
    // Command, Result, Request i Response ne smeju imati ValueObject tipove jer su oni rezervisani za Domain

    public class CreateFlightEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/flight", async (CreateFlightRequestDto request, ISender sender, CancellationToken ct) =>
            {
                var command = request.FromCreateFlightRequestToCreateFlightCommand();

                var result = await sender.Send(command, ct);

                var response = result.FromCreateFlightResultToCreateFlightResponse();

                return Results.Ok(response);
            })
            .RequireAuthorization() // dodaj ako treba rolu 
            .WithName("CreateFlight")
            .Produces<CreateFlightResponseDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Flight")
            .WithDescription("Create Flight");
        }
    }
}
