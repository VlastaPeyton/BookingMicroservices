
using System.Net;

namespace BuildingBlocks.Exceptions
{
    // Zbog DDD razdvojim Domain od Application logike, pa tako i za exceptions

    public class DomainException : Exception
    {
        public DomainException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest, int? code = null)
          : base(message) { }

        public DomainException(string message, Exception innerException, HttpStatusCode statusCode = HttpStatusCode.BadRequest, int? code = null)
            : base(message, innerException) { }
    }
}
