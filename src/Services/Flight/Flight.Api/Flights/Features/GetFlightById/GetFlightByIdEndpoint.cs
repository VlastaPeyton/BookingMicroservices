using Carter;
using Flight.Api.Flights.Dtos;
using Flight.Api.Flights.Mappers;
using MediatR;

namespace Flight.Api.Flights.Features.GetFlightById
{   
    // Mislim da necu koristiti ovaj endpoint, jer GetFlightByIdQueryHandler pozivam kroz FlightGrpcService
    public record GetFlightByIdResponseDto(FlightDto FlightDto);
    public class GetFlightByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/flight/{flightId}", async (Guid flightId, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetFlightByIdQuery(flightId), ct);

                var response = result.FromGetFlightByIdRequestToResponse();

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetFlightById")
            .Produces<GetFlightByIdResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Flight By Id")
            .WithDescription("Get Flight By Id");
        }
    }
}
