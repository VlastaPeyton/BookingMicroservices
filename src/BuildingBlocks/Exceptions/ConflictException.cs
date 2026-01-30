using System.Net;

namespace BuildingBlocks.Exceptions
{
    public class ConflictException : AppException
    {
        public ConflictException(string message, int? code = null) : base(message, HttpStatusCode.Conflict, code)
        {
        }
    }
}
