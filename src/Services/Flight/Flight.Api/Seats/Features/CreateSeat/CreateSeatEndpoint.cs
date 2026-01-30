using Carter;
using Flight.Api.Seats.Enums;
using Flight.Api.Seats.Mappers;
using MediatR;

namespace Flight.Api.Seats.Features.CreateSeat
{   
    public record CreateSeatRequestDto(int SeatNumber, 
                                       SeatTypeEnum SeatType,  // U BB registrovao da string iz Http Request mapira u Enum iz RequestDto 
                                       SeatClassEnum SeatClass,
                                       Guid FlightId);
    public record CreateSeatResponseDto(Guid Id);

    public class CreateSeatEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/flight/create-seat", async (CreateSeatRequestDto request, ISender sender, CancellationToken ct) =>
            {
                var command = request.FromCreateSeatRequestToCommand();

                var result = await sender.Send(command, ct);

                var response = result.FromCreateSeatResultToResponse();

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("CreateSeat")
            .Produces<CreateSeatResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Seat")
            .WithDescription("Create Seat");
        }
    }
}
