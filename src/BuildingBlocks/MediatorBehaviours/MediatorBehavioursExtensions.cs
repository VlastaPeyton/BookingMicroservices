using System.Reflection;
using BuildingBlocks.MediatorBehaviours.Logging;
using BuildingBlocks.MediatorBehaviours.Validation;
using EasyCaching.Core.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.MediatorBehaviours
{
    public static class MediatorBehavioursExtensions
    {   
        // ne ubacaj CachingBehavior, jer za sada ReadModel u Mongo radi odlicno za Query + nije genericki 
        // Ne koristi onda ovu metodu 
        public static void AddEasyCaching(this IServiceCollection services, Assembly assembly)
        {
            services.AddEasyCaching(options =>
            {
                options.UseRedis(config =>
                {
                    config.DBConfig.Endpoints.Add(new ServerEndPoint("localhost", 6379));
                    config.DBConfig.Database = 0; // Koju Redis bazu koristim od 0-15 izbora
                                                  // config.DBConfig.Password = "password"; // ako treba
                }, "redis");
            });
        }

        // AddMediatR dodajem u mikroservis jer je to built-in metoda

    }
}
