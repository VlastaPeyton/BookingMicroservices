using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace BuildingBlocks.Jwt
{
    // Extension metode za konfiguraciju JWT autentikacije
    public static class JwtExtensions
    {
        public static IServiceCollection AddJwt(this IServiceCollection services, IConfiguration configuration)
        {
            // Procitaj JWT opcije iz appsettings.json
            var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>();
            if (jwtOptions is null)
                throw new InvalidOperationException("JWT configuration is missing in appsettings.json");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.Authority = jwtOptions.Authority; // IdentityServer microservice url 
                        options.Audience = jwtOptions.Audience;  // Microservis ciji endpoint pozivam

                        // Za development moze biti false, za production mora biti true
                        options.RequireHttpsMetadata = false;

                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuers = [jwtOptions.Authority], // Mnozina daje fleksibilnost 

                            ValidateAudience = true,
                            ValidAudiences = [jwtOptions.Audience], // Mnozina daje fleksibilnost

                            ValidateLifetime = true,

                            ValidateIssuerSigningKey = true, // Discovery endpoint automatski ugradjen u IdentitServer 

                            // Mapiranje zeljenih claimova iz default dugackog u moderno i kratko ime
                            NameClaimType = "name",
                            RoleClaimType = "role" // Zbog [Authorize(Role="Admin/User")]
                        };

                        // Razbij scope claim na pojedinacne claimove, jer mora tako
                        options.Events = new JwtBearerEvents
                        {
                            OnTokenValidated = context =>
                            {
                                if (context.Principal?.Identity is ClaimsIdentity identity)
                                {
                                    var scopeClaim = identity.FindFirst("scope");
                                    if (scopeClaim != null)
                                    {
                                        var scopes = scopeClaim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                                        identity.RemoveClaim(scopeClaim);

                                        foreach (var scope in scopes)
                                        {
                                            identity.AddClaim(new Claim("scope", scope));
                                        }
                                    }
                                }
                                return Task.CompletedTask;
                            }
                        };
                    });

            // Zbog RequireAuthorization() u endpoints vrsi se provera zeljenih claims IdentityServer microservice napravi jwt
            services.AddAuthorization(options =>
            {
                // ApiScope claim policy 
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", jwtOptions.Audience); // Audience = microservis ciji endpoint pozivam + proverava "scope" claim u jwt
                });

                // Role-based admin claim policy
                options.AddPolicy(Roles.Roles.Admin, policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireRole(Roles.Roles.Admin); // Proverava "role" claim in jwt da l sadrzi "admin"
                });

                // Role-based user claim policy
                options.AddPolicy(Roles.Roles.User, policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireRole(Roles.Roles.User); // Proverava "role" claim in jwt da l sadrzi "user"
                });
            });

            return services;
        }

        // Samo u microservice1 koji putem REST API poziva microservice2
        public static IServiceCollection AddHttpClientWithAuth<TInterface, TImplementation>(this IServiceCollection services, string baseUrl)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            services.AddHttpContextAccessor();
            services.AddTransient<AuthorizationHeaderHandler>();

            services.AddHttpClient<TInterface, TImplementation>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            })
            .AddHttpMessageHandler<AuthorizationHeaderHandler>();

            return services;
        }
    }
}
