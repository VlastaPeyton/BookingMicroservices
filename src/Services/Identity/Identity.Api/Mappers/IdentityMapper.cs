using Identity.Api.Features.RegisterUser;

namespace Identity.Api.Mappers
{
    public static class IdentityMapper
    {
        public static RegisterUserCommand FromRegisterUserRequestToCommand(this RegisterUserRequestDto request)
        {   // Zelim da baza tj Identity dodeli Guid za novog usera
            return new RegisterUserCommand(request.FirstName,
                                           request.LastName,
                                           request.Username,
                                           request.Email,
                                           request.Password,
                                           request.ConfirmPassword,
                                           request.PassportNumber);
                                          
        }

        public static RegisterUserResponseDto FromRegisterUserResultToResponse(this RegisterUserResult result)
        {
            return new RegisterUserResponseDto(result.Id,
                                               result.FirstName,
                                               result.LastName,
                                               result.Username,
                                               result.PassportNumber);
        }
    }
}
