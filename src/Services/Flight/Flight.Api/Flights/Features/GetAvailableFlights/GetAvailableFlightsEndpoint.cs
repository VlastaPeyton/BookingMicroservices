using Carter;
using Flight.Api.Flights.Dtos;
using Flight.Api.Flights.Mappers;
using MediatR;

namespace Flight.Api.Flights.Features.GetAvailableFlights
{   
    public record GetAvailableFlightsResponseDto(IEnumerable<FlightDto> FlightDtos);
    public class GetAvailableFlightsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/flight", async (ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetAvailableFlightsQuery(), ct);

                var response = result.FromGetAvailableFlightsResultToResponse();

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetAvailableFlights")
            .Produces<GetAvailableFlightsResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Available Flights")
            .WithDescription("Get Available Flights");
        }
    }
}
