using Carter;
using MediatR;

namespace Flight.Api.Flights.Features.DeleteFlight
{   
    public class DeleteFlightEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/flight", async (Guid flightId, ISender sender, CancellationToken ct) =>
            {
                await sender.Send(new DeleteFlightCommand(flightId), ct);

                return Results.NoContent();
            })
            .RequireAuthorization()
            .WithName("DeleteFlight")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Delete Flight")
            .WithDescription("Delete Flight");
        }
    }
}
