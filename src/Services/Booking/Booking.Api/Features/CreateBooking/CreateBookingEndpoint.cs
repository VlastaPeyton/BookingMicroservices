using Booking.Api.Mapper;
using Carter;
using MediatR;

namespace Booking.Api.Features.CreateBooking
{   
    public record CreateBookingRequestDto(Guid PassengerId, Guid FlightId, string Description);
    public record CreateBookingResponseDto(Guid Id);
    // Command, Result, Request i Response ne smeju imati ValueObject tipove jer su oni rezervisani za Domain

    public class CreateBookingEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {   // https://localhost:port/booking POST
            app.MapPost("/booking", async (CreateBookingRequestDto request, ISender sender, CancellationToken ct) =>
            {
                var command = request.FromCreateBookingRequestToCreateBookingCommand();

                var result = await sender.Send(command, ct);

                var response = result.FromCreateBookingResultToCreateBookingResponse();

                return Results.Ok(response);
            })
              .RequireAuthorization() // Isto ko [Authorize] u controller, dodaj u Program.cs policy i onda i ovde
              .WithName("CreateBooking")
              .Produces<CreateBookingResponseDto>()
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create Booking")
              .WithDescription("Create Booking");
        }
    }
}
