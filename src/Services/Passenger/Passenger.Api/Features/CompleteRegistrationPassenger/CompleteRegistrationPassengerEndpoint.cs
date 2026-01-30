using Carter;
using MediatR;
using Passenger.Api.Dtos;
using Passenger.Api.Enums;
using Passenger.Api.Mappers;

namespace Passenger.Api.Features.CompleteRegistrationPassenger
{   
    public record CompleteRegistrationPassengerRequestDto(string PassportNumber, 
                                                          PassengerTypeEnum PassengerType, // U BB registrovao da string iz Http Request mapira u Enum iz RequestDto 
                                                          int Age);
    public record CompleteRegistrationPassengerResponseDto(PassengerDto PassengerDto);

    public class CompleteRegistrationPassengerEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("passenger/complete-registration", async (CompleteRegistrationPassengerRequestDto request, ISender sender, CancellationToken ct) =>
            {
                var command = request.FromCompleteRegistrationPassengerRequestToResult();

                var result = await sender.Send(command, ct);

                var response = result.FromCompleteRegistrationPassengerResultToResponse();

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("CompleteRegisterPassenger")
            .Produces<CompleteRegistrationPassengerResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Complete Register Passenger")
            .WithDescription("Complete Register Passenger");
        }
    }
}
