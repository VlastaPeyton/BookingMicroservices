
using MediatR;

namespace BuildingBlocks.Core.Events
{
    public interface IDomainEvent : INotification
    {
        Guid EventId { get; }
        DateTime OccurredOn { get; }
    }
}
