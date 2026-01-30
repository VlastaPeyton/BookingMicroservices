using BuildingBlocks.Core.Events;

namespace BuildingBlocks.EventStore.EventTypeMappers
{   
    public class EventTypeMapper : IEventTypeMapper
    {
        private readonly Dictionary<string, Type> _domainEventTypes = new();

        public EventTypeMapper() 
        {
            // On app startup, skeniraj sve event tipove
            ScanAllDomainEventTypesOnAppStartup(); 
        }

        public void ScanAllDomainEventTypesOnAppStartup() 
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var domainEventTypes = assembly.GetTypes()
                                               .Where(t => typeof(IDomainEvent).IsAssignableFrom(t)
                                                          && !t.IsInterface
                                                          && !t.IsAbstract);

                foreach (var domainEventType in domainEventTypes)
                {
                    _domainEventTypes[domainEventType.Name] = domainEventType;
                }
            }
        }

        public Type GetDomainEventType(string domainEventTypeName)
        {
            if (_domainEventTypes.TryGetValue(domainEventTypeName, out var type))
                return type;

            throw new InvalidOperationException($"Event type '{domainEventTypeName}'");
        }
    }
}
