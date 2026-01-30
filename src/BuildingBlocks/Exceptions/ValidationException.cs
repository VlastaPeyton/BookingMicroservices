using System.Net;

namespace BuildingBlocks.Exceptions
{
    public class ValidationException : AppException
    {
        public ValidationException(string message, int? code = null) : base(message, HttpStatusCode.BadRequest, code)
        {
        }
    }
}
