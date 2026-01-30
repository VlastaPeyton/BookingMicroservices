using Carter;
using Flight.Api.Aircrafts.Mappers;
using MediatR;

namespace Flight.Api.Aircrafts.Features.CreateAircraft
{
    public record CreateAircraftRequestDto(string Name, string Model, int ManufacturingYear);
    public record CreateAircraftResponseDto(Guid Id);
    // Command, Result, Request i Response ne smeju imati ValueObject tipove jer su oni rezervisani za Domain

    public class CreateAircraftEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {   // https://localhost:port/aircraft POST
            app.MapPost("/aircraft", async (CreateAircraftRequestDto request, ISender sender, CancellationToken ct) =>
            {
                var command = request.FromCreateAircraftRequestToCreateAircraftCommand();

                var result = await sender.Send(command, ct);

                var response = result.FromCreateAircraftResultToCreateAircraftResponse();

                return Results.Ok(response);
            })
            .RequireAuthorization() // Dopuni ako treba neki role claims posebni
            .WithName("CreateAircraft")
            .Produces<CreateAircraftResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Aircraft")
            .WithDescription("Create Aircraft");
        }
    }
}
