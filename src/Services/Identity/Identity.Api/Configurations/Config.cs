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
            new IdentityResources.OpenId(),  // user id
            new IdentityResources.Profile(), // ime, prezime, slika...
            new IdentityResources.Email()
        };

        // User Roles in desired Scope 
        public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope>
        {
            new ApiScope(StandardApiScopes.FlightApi),     // "flight-api"
            new ApiScope(StandardApiScopes.PassengerApi),  
            new ApiScope(StandardApiScopes.BookingApi),    
            new ApiScope(StandardApiScopes.IdentityApi),   
            new ApiScope(JwtClaimTypes.Role, new List<string> {"role"})  // Omoguci role claim da se pojavi u access token jwt da bi povezalo sa [Authorize(Roles = "...")]
        };

        // Koji APIs postoje i koje scope koriste
        public static IList<ApiResource> ApiResources => new List<ApiResource>
        {
            new ApiResource(StandardApiScopes.FlightApi)
            {
                Scopes = { StandardApiScopes.FlightApi }
            },
            new ApiResource(StandardApiScopes.PassengerApi)
            {
                Scopes = { StandardApiScopes.PassengerApi }
            },
            new ApiResource(StandardApiScopes.BookingApi)
            {
                Scopes = { StandardApiScopes.BookingApi }
            },
            new ApiResource(StandardApiScopes.IdentityApi)
            {
                Scopes = { StandardApiScopes.IdentityApi }
            },
        };

        // Koje FE mogu da traze token i sta smeju
        public static IEnumerable<Client> Clients => new List<Client>
        {
            new Client
            {
                ClientId = "client",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    JwtClaimTypes.Role, // Include roles scope
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
