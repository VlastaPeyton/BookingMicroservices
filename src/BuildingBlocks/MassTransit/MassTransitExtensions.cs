using System.Reflection;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.MassTransit
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection AddCustomMassTransit<TDbContext>(this IServiceCollection services, 
                                                                          IConfiguration configuration, 
                                                                          params Assembly[] consumerAssemblies)
            where TDbContext : DbContext
        {
            /* <TDbContext> jer mikroservisi nemaju isto ime za AppDbContext, pa da odredi u runtime
              consumerAssembly != null za RabbitMQ Publisher microservice
               consumerAsembly = null za RabbitMQ Consumer microservice 
               U RabbitMQ Publisher microservice Program.cs mora builder.Services.AddMassTransitRabbitMQAndOutbox(builder.Configuration) 
               U RabbitMQ Consumer microservice Program.cs mora builder.Services.AddMassTransitRabbitMQAndOutboxInbox(builder.Configuration, Assembly.GetExecutingAssembly()) 
               Consumer/Publisher ako samo prima/salje, treba imati samo Inbox/Outbox, ali ako radi oba, pa na osnovu toga u OnModelCreatign biram da l cu napisati both Inobx-Outbox ili samo jedno 
             */

            // Registruj RabbitMqOptions iz appsettings.json
            services.Configure<RabbitMqOptions>(configuration.GetSection(nameof(RabbitMqOptions)));

            services.AddMassTransit(x =>
            {   
                // Registruj Outbox + Inbox pipeline
                x.AddEntityFrameworkOutbox<TDbContext>(o =>
                {
                    o.QueryDelay = TimeSpan.FromSeconds(5); // Koliko cesto MassTransit built-in background worker proverava ima li noviteta u Outbox tabeli
                    o.UseSqlServer(); // Jer SQL Server koristim i za Consumer i za Publisher
                    o.UseBusOutbox(); // IPublishEndpoint.Publish(integrationEvent) ce samo upisati Integration Event u Outbox on Publisher side, dok MassTransit background worker ce periodicno proveravati Outbox i slati odatle u RabbitMQ
                    
                    // U OnModelCreating sam morao omoguciti Inbox i/ili Outbox tabele
                });
                /* Nakon ovoga, dopuni OnModelCreating, pa uradi Migraciju sada da bi se Inbox/Outbox tabele napravile u bazi
                  OnModelCreating za Publisher imace samo Outbox tabele jer gistro taj microservice ce slati samo u RabbitMQ, a nece da prima
                  OnModelCreating za Consumer imace samo Inbox tabele jer gistro taj microservice ce primati samo iz RabbitMQ,a nece slati 
                 */

                // RabbitMQ by default ima "ime_endpoint", ali lepse je "ime-endpoint" 
                x.SetKebabCaseEndpointNameFormatter();

                // Registruj sve Consumers 
                x.AddConsumers(consumerAssemblies);

                x.UsingRabbitMq((context, cfg) =>
                {
                    // Probaj prvo Aspire connection string 
                    var aspireConnectionString = configuration.GetConnectionString("rabbitmq");
                    if (!string.IsNullOrEmpty(aspireConnectionString))
                    { 
                        cfg.Host(new Uri(aspireConnectionString));
                    }
                    else
                    {
                        // Inace koristi RabbitMqOptions iz appsettings
                        var options = configuration.GetSection(nameof(RabbitMqOptions)).Get<RabbitMqOptions>();
                        cfg.Host(options?.HostName ?? "localhost", options?.Port ?? 5672, "/", host =>
                        {
                            host.Username(options?.UserName ?? "guest");
                            host.Password(options?.Password ?? "guest");
                        });
                    }

                    // Pravi queue za svakog consumera
                    cfg.ConfigureEndpoints(context);

                    // Retry konfiguracija 
                    cfg.UseMessageRetry(r =>
                    {
                        r.Exponential(retryLimit: 3,
                                      minInterval: TimeSpan.FromMilliseconds(200),
                                      maxInterval: TimeSpan.FromMinutes(2),
                                      intervalDelta: TimeSpan.FromMilliseconds(200));
                    });
                });
            });
            
            return services;
        }
    }
}
