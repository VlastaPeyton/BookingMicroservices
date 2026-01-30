using System.Net;

namespace BuildingBlocks.Exceptions
{
    public class BadRequestException : AppException
    {
        public BadRequestException(string message, int? code = null) : base(message, HttpStatusCode.BadRequest, code)
        {
        }
    }
}
