using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.EfCore
{
    public static class EfCoreExtensions
    {
        public static IServiceCollection AddApplicationDbContext<TContext>(this IServiceCollection services,
                                                                           string connectionString) 
            where TContext : ApplicationDbContextBase
        {
            services.AddDbContext<TContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IApplicationDbContext>(sp =>
                sp.GetRequiredService<TContext>());

            return services;
        }
    }
}
