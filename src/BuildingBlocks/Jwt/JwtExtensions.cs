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
                        options.Audience = jwtOptions.Audience;  // Trenutni microservis

                        // Za development moze biti false, za production mora biti true
                        options.RequireHttpsMetadata = false;

                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuers = [jwtOptions.Authority], // Mnozina daje fleksibilnost 

                            ValidateAudience = true,
                            ValidAudiences = [jwtOptions.Audience], // Mnozina daje fleksibilnost

                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.FromSeconds(5), // Dozvoli 5s razlike u vremenu

                            ValidateIssuerSigningKey = true, // Discovery endpoint automatski ugradjen u IdentitServer 
                        };   
                    });

            services.AddAuthorization(options =>
            {
                // Policy za API scope - zahteva autentikovanog usera sa odgovarajucim scope-om
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", jwtOptions.Audience);
                });

                // Role-based polise
                options.AddPolicy(Roles.Roles.Admin, policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireRole(Roles.Roles.Admin);
                });

                options.AddPolicy(Roles.Roles.User, policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireRole(Roles.Roles.User);
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
