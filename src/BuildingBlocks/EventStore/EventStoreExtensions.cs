using BuildingBlocks.EventStore.Checkpoints;
using BuildingBlocks.EventStore.Domain;
using BuildingBlocks.EventStore.EventTypeMappers;
using BuildingBlocks.EventStore.Repositories.EvetnStore;
using BuildingBlocks.EventStore.Repositories.Snapshots;
using BuildingBlocks.EventStore.Subscriptions;
using EventStore.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace BuildingBlocks.EventStore
{   
    // U microservisima pozivam ovo
    public static class EventStoreExtensions
    {
        public static IServiceCollection AddEventStore(this IServiceCollection services, IConfiguration configuration)
        {
            // EventStoreClient
            var eventStoreConnectionString = configuration.GetConnectionString("EventStore") ?? "esdb://localhost:2113?tls=false";
            var settings = EventStoreClientSettings.Create(eventStoreConnectionString);
            var eventStoreClient = new EventStoreClient(settings);
            services.AddSingleton(eventStoreClient);

            return services;
        }

        public static IServiceCollection AddMongo(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoConnectionString = configuration.GetConnectionString("Mongo") ?? "mongodb://localhost:27017";
            var mongoDatabaseName = configuration["MongoDb:DatabaseName"] ?? "EventStoreSupportMongo";
            var mongoClient = new MongoClient(mongoConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDatabaseName);
            services.AddSingleton<IMongoClient>(mongoClient);
            services.AddSingleton(mongoDatabase);

            return services;
        }

        public static IServiceCollection AddEventTypeMapperAndCheckpoint(this IServiceCollection services)
        {
            services.AddSingleton<IEventTypeMapper, EventTypeMapper>();
            services.AddSingleton<ICheckpointStore, CheckpointStore>();

            return services;
        }

        public static IServiceCollection AddEventStoreRepositories<TAggregate, TId>(this IServiceCollection services)
            where TAggregate : AggregateRootEventSource<TId>, new()
            where TId : IPersistableId
        {
            services.AddScoped<ISnapshotRepository<TAggregate, TId>, SnapshotRepository<TAggregate, TId>>();
            services.AddScoped<IEventStoreRepository<TAggregate, TId>, EventStoreRepository<TAggregate, TId>>();

            return services;
        }

        public static IServiceCollection AddEventStoreSubscriptionToAll(this IServiceCollection services, string subscriptionId= "default-subscription")
        {
            // AddHostedService jer SubscriptionToEventStore : BackgroundService
            services.AddHostedService(sp => new SubscriptionToEventStore(sp.GetRequiredService<EventStoreClient>(),
                                                                         sp.GetRequiredService<IEnumerable<IDomainEventHandlerES>>(),
                                                                         sp.GetRequiredService<ICheckpointStore>(),
                                                                         sp.GetRequiredService<IEventTypeMapper>(),
                                                                         subscriptionId));

            return services;
        }
        // Biram AddEventStoreSubscriptionToAll ili AddEventStoreSubscriptionToStreams jer mala je sansa da u 1 mikroservis imam razlicite subscriptions
        public static IServiceCollection AddEventStoreSubscriptionToStreams(this IServiceCollection services, string subscriptionId, params string[] streamNames)
        {
            // AddHostedService jer SubscriptionToEventStore : BackgroundService
            services.AddHostedService(sp => new SubscriptionToEventStore(sp.GetRequiredService<EventStoreClient>(),
                                                                         sp.GetRequiredService<IEnumerable<IDomainEventHandlerES>>(),
                                                                         sp.GetRequiredService<ICheckpointStore>(),
                                                                         sp.GetRequiredService<IEventTypeMapper>(),
                                                                         subscriptionId,
                                                                         streamNames));

            return services;
        }

        // Rucna registracija svakog CustomDomainEventHandler ili CustomProjectionHandler
        public static IServiceCollection AddDomainEventHandler<THandler>(this IServiceCollection services)
            where THandler : class, IDomainEventHandlerES
        {
            services.AddScoped<IDomainEventHandlerES, THandler>();
            return services;
        }

        // Svaki mikroservis (.csproj) koji ima neki CustomDomainEventHandler ili CustomProjectionhandler, samo prosledim bilo koju klasu iz tog projekta i nadje sve sto mi treba
        public static IServiceCollection AddDomainEventHandlersFromAssembly<TMarker>(this IServiceCollection services)
        {
            var assembly = typeof(TMarker).Assembly;

            var handlerTypes = assembly.GetTypes().Where(t => typeof(IDomainEventHandlerES).IsAssignableFrom(t)
                                                              && !t.IsInterface
                                                              && !t.IsAbstract);

            foreach (var handlerType in handlerTypes)
                services.AddScoped(typeof(IDomainEventHandlerES), handlerType);

            return services;
        }
    }
}