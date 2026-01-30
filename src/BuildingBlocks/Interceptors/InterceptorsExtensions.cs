

using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Interceptors
{   
    public static class InterceptorsExtensions
    {   
        // Necu koristiti, jer AuditInterceptor ne uklapa se u ApplicationDbContextBase UoW koncept !
        public static void AddAuditInterceptor(this IServiceCollection services)
        {
            services.AddScoped<AuditInterceptor>();
            // Nakon ovoga idem u EfCoreExtensions da dodam interceptor from DI u DbContext
        }

        // Necu koristiti, jer DispatchDomainEventsInterceptor ne uklapa se u ApplicationDbContextBase UoW koncept ! 
        public static void AddDispatchDomainEventsInterceptor(this IServiceCollection services)
        {
            services.AddScoped<DispatchDomainEventsInterceptor>();
        }
    }
}
