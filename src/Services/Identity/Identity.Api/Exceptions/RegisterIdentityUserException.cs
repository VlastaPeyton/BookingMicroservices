using BuildingBlocks.Exceptions;

namespace Identity.Api.Exceptions
{
    public class RegisterIdentityUserException : AppException
    {
        public RegisterIdentityUserException(string message) : base(message)
        {
        }
    }
}
