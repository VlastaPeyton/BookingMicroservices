

namespace BuildingBlocks.EventStore.EventTypeMappers
{
    public interface IEventTypeMapper
    {
        void ScanAllDomainEventTypesOnAppStartup();
        Type GetDomainEventType(string domainEventTypeName);
    }
}
