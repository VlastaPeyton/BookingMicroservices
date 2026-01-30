using Flight.Api.Flights.Models;
using Flight.Api.Seats.Events.DomainEvents;
using Flight.Api.Seats.Models;
using MediatR;
using MongoDB.Driver;

namespace Flight.Api.Seats.DomainEventHandlers
{
    public class SeatReservedDomainEventHandler : INotificationHandler<SeatReservedDomainEvent>
    {
        private readonly IMongoCollection<SeatReadModel> _seatReadModelCollection;

        public SeatReservedDomainEventHandler(IMongoDatabase mongoDatabase)
        {
            _seatReadModelCollection = mongoDatabase.GetCollection<SeatReadModel>("SeatReadModels");
        }

        public async Task Handle(SeatReservedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var filter = Builders<SeatReadModel>.Filter.Eq(s => s.SeatNumber, domainEvent.SeatNumber);

            var update = Builders<SeatReadModel>.Update.Set(s => s.IsDeleted, domainEvent.IsDeleted);

            await _seatReadModelCollection.UpdateOneAsync(filter,
                                                          update,
                                                          new UpdateOptions { IsUpsert = false }, // Ako dokument ne postoji, ne kreira novi
                                                           cancellationToken);
        }
    }
}
