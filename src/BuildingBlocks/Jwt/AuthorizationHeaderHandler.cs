using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Jwt
{
    // DelegatingHandler koji propagira Authorization header kroz HTTP pozive
    // Koristi se kada microservice1 samo putem HTTP(REST API) poziva microservice2, gde automatski prenosim JWT token u servis koji je indirektno pozvan
    public class AuthorizationHeaderHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor; // Mora builder.Services.AddHttpContextAccessor()

        public AuthorizationHeaderHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Uzmi Authorization header iz trenutnog HTTP zahteva
            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString(); // jwt token

            if (!string.IsNullOrEmpty(authHeader))
            {
                // Ocisti "Bearer " prefix ako postoji i dodaj na odlazeci zahtev
                var token = authHeader.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
