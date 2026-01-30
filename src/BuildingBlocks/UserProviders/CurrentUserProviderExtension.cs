using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.UserProviders
{
    public static class CurrentUserProviderExtension
    {
        public static IServiceCollection AddCurrentUserProvider(this IServiceCollection services)
        {
            services.AddHttpContextAccessor(); // Zbog IHttpContextAccessor koji koristim u CurrentUserProvider
            services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

            return services;
        }
    }
}
