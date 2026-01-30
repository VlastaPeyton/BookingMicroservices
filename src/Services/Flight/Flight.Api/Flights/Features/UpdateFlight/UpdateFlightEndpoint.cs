using Carter;
using Flight.Api.Flights.Enums;
using Flight.Api.Flights.Mappers;
using MediatR;

namespace Flight.Api.Flights.Features.UpdateFlight
{   
    public record UpdateFlightRequestDto(Guid FlightId,  // FE mora reci koji FlightId zelim da azuriram
                                        string FlightNumber, 
                                        Guid AircraftId, 
                                        Guid DepartureAirportId,
                                        DateTime DepartureDate, DateTime ArriveDate,
                                        Guid ArriveAirportId, 
                                        decimal DurationMinutes, 
                                        DateTime FlightDate, 
                                        FlightStatusEnum Status, // U BB registrovao da string iz Http Request mapira u Enum iz RequestDto 
                                        decimal Price,
                                        bool IsDeleted);
    public class UpdateFlightEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/flight", async (UpdateFlightRequestDto request, ISender sender, CancellationToken ct) =>
            {
                var command = request.FromUpdateFlightRequestToUpdateFlightCommand();

                var result = await sender.Send(command, ct);

                return Results.NoContent();
            })
            .RequireAuthorization()
            .WithName("UpdateFlight")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Update Flight")
            .WithDescription("Update Flight");
        }
    }
}