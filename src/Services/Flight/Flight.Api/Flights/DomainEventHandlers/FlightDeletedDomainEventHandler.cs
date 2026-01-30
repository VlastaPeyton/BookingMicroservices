using Flight.Api.Flights.Events.DomainEvents;
using Flight.Api.Flights.Models;
using MediatR;
using MongoDB.Driver;

namespace Flight.Api.Flights.DomainEventHandlers
{
    /* U Booking microservice imam DomainEventHandler koji se aktivira automatski jer je povezan na EventStore Subscriber 
      ali Flight microservice nije EventSourcing, pa ovaj handler se aktivira automatski zbog MediatR jer 
      FlightDbContext je nasledio ApplicationDbContextBase koji pomocu MediatR u CommitTransactionAsync dispatch domainevent. */
    public class FlightDeletedDomainEventHandler : INotificationHandler<FlightDeletedDomainEvent>
    {
        private readonly IMongoCollection<FlightReadModel> _flightReadModelCollection;

        public FlightDeletedDomainEventHandler(IMongoDatabase mongoDatabase)
        {
            _flightReadModelCollection = mongoDatabase.GetCollection<FlightReadModel>("FlightReadModels");
        }

        public async Task Handle(FlightDeletedDomainEvent domainEvent, CancellationToken cancellationToken)
        {

            var filter = Builders<FlightReadModel>.Filter.Eq(f => f.FlightId, domainEvent.FlightId);

            var update = Builders<FlightReadModel>.Update.Set(f => f.IsDeleted, true); // Samo ovo polje azuriram za soft delete

            await _flightReadModelCollection.UpdateOneAsync(filter,
                                                            update,
                                                            new UpdateOptions { IsUpsert = false }, // soft delete -> ne kreiramo novi dokument
                                                            cancellationToken
                                                            );
        }
    }
}
