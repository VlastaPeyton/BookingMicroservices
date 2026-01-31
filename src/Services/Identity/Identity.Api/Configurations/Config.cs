using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Identity.Api.Scopes;
using Microsoft.VisualBasic;

namespace Identity.Api.Configurations
{
    public static class Config
    {   
        // Koje user info su ukljucene u id token za OpenID
       public static IEnumerable<IdentityResource> IdentityResources => new List<IdentityResource>
        {
            new IdentityResources.OpenId(),  // "sub" claim tj user id
            new IdentityResources.Profile(), // "ime", "prezime", "slika" claim ...
            new IdentityResources.Email(),   // "email" claim
            new IdentityResource("roles", "User Roles", new[] { "role" })  // "role" claim
        };

        // Sta klijent sme da radi 
        public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope>
        {
            new ApiScope(StandardApiScopes.FlightApi, "Flight API"),     // "flight-api"
            new ApiScope(StandardApiScopes.PassengerApi, "Passenger API"),  // "passenger-api"
            new ApiScope(StandardApiScopes.BookingApi, "Booking API"),    // "booking-api"
            new ApiScope(StandardApiScopes.IdentityApi, "Identity API"),   // "identity-api"
        };

        // Koji claims idu u token za odredjeni mikroservis
        public static IList<ApiResource> ApiResources => new List<ApiResource>
        {
            new ApiResource(StandardApiScopes.FlightApi)
            {
                Scopes = { StandardApiScopes.FlightApi }, // Koji scopes pripadaju ovom apiju
                UserClaims = { "role", "name" }           // Koji user claims idu u token
            },
            new ApiResource(StandardApiScopes.PassengerApi)
            {
                Scopes = { StandardApiScopes.PassengerApi },
                UserClaims = { "role", "name" }
            },
            new ApiResource(StandardApiScopes.BookingApi)
            {
                Scopes = { StandardApiScopes.BookingApi },
                UserClaims = { "role", "name" }
            },
            new ApiResource(StandardApiScopes.IdentityApi)
            {
                Scopes = { StandardApiScopes.IdentityApi },
                UserClaims = { "role", "name" }
            },
        };

        // Koje FE mogu da traze token i sta smeju
        public static IEnumerable<Client> Clients => new List<Client>
        {
            new Client
            {
                ClientId = "client",
                AllowedGrantTypes = GrantTypes.Code,
                ClientSecrets ={ new Secret("secret".Sha256()) },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "roles",
                    StandardApiScopes.FlightApi,
                    StandardApiScopes.PassengerApi,
                    StandardApiScopes.BookingApi,
                    StandardApiScopes.IdentityApi,
                },
                AccessTokenLifetime = 3600,  // authorize the client to access protected resources
                IdentityTokenLifetime = 3600, // authenticate the user,
                AlwaysIncludeUserClaimsInIdToken = true // Include claims in ID token
            }
        };
    }
}
