using Carter;
using Flight.Api.Airports.Mappers;
using MediatR;

namespace Flight.Api.Airports.Features.CreateAirport
{   
    public record CreateAirportRequestDto(string Name, string Address, string Code);
    public record CreateAirportResponseDto(Guid Id);
    // Command, Result, Request i Response ne smeju imati ValueObject tipove jer su oni rezervisani za Domain

    public class CreateAirportEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/airport", async (CreateAirportRequestDto request, ISender sender, CancellationToken ct) =>
            {
                var command = request.FromCreateAirportRequestToCreateAirportCommand();

                var result = await sender.Send(command, ct);

                var response = result.FromCreateAirportResultToCreateAirportResponse();

                return Results.Ok(response);
            })
              .RequireAuthorization() // Dodaj neku role autorizaciju ako treba
              .WithName("CreateAirport")
              .Produces<CreateAirportResponseDto>()
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create Airport")
              .WithDescription("Create Airport");
        }
    }
}
