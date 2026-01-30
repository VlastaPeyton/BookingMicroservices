

using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.UserProviders
{   
    // U Minimal Api endpoint / Controller mogu direktno HttpContext, ali van toga IHttpContextAccessor mora
    public class CurrentUserProvider : ICurrentUserProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? GetCurrentUserId()
        {
            return _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier); // cita iz JWT a tamo je svaki claim string

        }
    }
}
