

namespace BuildingBlocks.Core.Events
{
    public interface IIntegrationEvent
    {
        Guid EventId { get; }
        DateTime OccurredOn { get; }
    }
}
