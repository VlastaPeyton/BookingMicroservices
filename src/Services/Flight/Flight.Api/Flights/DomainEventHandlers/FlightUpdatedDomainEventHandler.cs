using Flight.Api.Flights.Events.DomainEvents;
using Flight.Api.Flights.Models;
using MediatR;
using MongoDB.Driver;

namespace Flight.Api.Flights.DomainEventHandlers
{    /* U Booking microservice imam DomainEventHandler koji se aktivira automatski jer je povezan na EventStore Subscriber 
      ali Flight microservice nije EventSourcing, pa ovaj handler se aktivira automatski zbog MediatR jer 
      FlightDbContext je nasledio ApplicationDbContextBase koji pomocu MediatR u CommitTransactionAsync dispatch domainevent. */
    public class FlightUpdatedDomainEventHandler : INotificationHandler<FlightUpdatedDomainEvent>
    {
        private readonly IMongoCollection<FlightReadModel> _flightReadModelCollection;

        public FlightUpdatedDomainEventHandler(IMongoDatabase mongoDatabase)
        {
            _flightReadModelCollection = mongoDatabase.GetCollection<FlightReadModel>("FlightReadModels");
        }

        public async Task Handle(FlightUpdatedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var filter = Builders<FlightReadModel>.Filter.Eq(f => f.FlightId, domainEvent.FlightId);

            var update = Builders<FlightReadModel>.Update
                                                  .Set(f => f.FlightNumber, domainEvent.FlightNumber)
                                                  .Set(f => f.AircraftId, domainEvent.AircraftId)
                                                  .Set(f => f.DepartureDate, domainEvent.DepartureDate)
                                                  .Set(f => f.DepartureAirportId, domainEvent.DepartureAirportId)
                                                  .Set(f => f.ArriveDate, domainEvent.ArriveDate)
                                                  .Set(f => f.ArriveAirportId, domainEvent.ArriveAirportId)
                                                  .Set(f => f.DurationMinutes, domainEvent.DurationMinutes)
                                                  .Set(f => f.FlightDate, domainEvent.FlightDate)
                                                  .Set(f => f.Status, domainEvent.Status)
                                                  .Set(f => f.Price, domainEvent.Price)
                                                  .Set(f => f.IsDeleted, domainEvent.IsDeleted);

            await _flightReadModelCollection.UpdateOneAsync(filter,
                                                            update,
                                                            new UpdateOptions { IsUpsert = true }, // Ako dokument ne postoji, kreira novi
                                                            cancellationToken);
        }
    }
}
