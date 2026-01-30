using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Models
{
    public class User : IdentityUser<Guid>
    {
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required int PassPortNumber { get; init; }
    }
}
