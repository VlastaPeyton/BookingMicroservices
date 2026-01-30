using System.Net;

namespace BuildingBlocks.Exceptions
{
    public class NotFoundException : AppException
    {
        public NotFoundException(string message, int? code = null) : base(message, HttpStatusCode.NotFound, code)
        {
        }
    }
}
