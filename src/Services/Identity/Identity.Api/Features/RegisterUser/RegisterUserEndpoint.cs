using Carter;
using Identity.Api.Mappers;
using MediatR;

namespace Identity.Api.Features.RegisterUser
{   
    public record RegisterUserRequestDto(string FirstName,
                                         string LastName,
                                         string Username,
                                         string Email,
                                         string Password,
                                         string ConfirmPassword,
                                         int PassportNumber);
    public record RegisterUserResponseDto(Guid Id, 
                                          string FirstName, 
                                          string LastName, 
                                          string Username,
                                          int PassportNumber);
    public class RegisterUserEndpoint : ICarterModule
    {   

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/identity/register-user", async (RegisterUserRequestDto request, ISender sender, CancellationToken ct) =>
            {
                var command = request.FromRegisterUserRequestToCommand();

                var result = await sender.Send(command, ct);

                var response = result.FromRegisterUserResultToResponse();

                return Results.Ok(response);
            })
            .RequireAuthorization() // Nema jer ovom endpointu svi smeju pristupiti 
            .WithName("RegisterUser")
            .Produces<RegisterUserResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Register User")
            .WithDescription("Register User");
        }
    }
}
