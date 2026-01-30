using System.Net;

namespace BuildingBlocks.Exceptions
{   
    // Zbog DDD razdvojim Domain od Application logike, pa tako i za exceptions
    public class AppException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public int? Code { get; } 

        public AppException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest, int? code = null)
            : base(message)
        {
            StatusCode = statusCode;
            Code = code;
        }

        /* U catch bloku, ako ne zelim throw vec malo vise detalja, mogu da prosledim uhvaceni exception wrappovan u drugi exception cime 
         gubim stack, ali zadrzavam ga jer je uhvaceni exception wrappovan u novi. */
        public AppException(string message, Exception innerException, HttpStatusCode statusCode = HttpStatusCode.BadRequest, int? code = null)
            : base(message, innerException)
        {   
            StatusCode = statusCode;
            Code = code;
        }
    }
}
