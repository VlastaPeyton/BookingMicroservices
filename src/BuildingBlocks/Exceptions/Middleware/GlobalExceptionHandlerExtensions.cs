
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Exceptions.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IServiceCollection AddGlobalExceptionHandler(this IServiceCollection services)
        {
            services.AddTransient<GlobalExceptionHandlingMiddleware>(); // Zbog IMiddleware mora AddTransient
            return services;
        }

        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        }
    }
}
