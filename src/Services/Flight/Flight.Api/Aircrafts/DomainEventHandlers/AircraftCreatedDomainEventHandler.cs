using Flight.Api.Aircrafts.Events.DomainEvents;
using Flight.Api.Aircrafts.Models;
using MediatR;
using MongoDB.Driver;

namespace Flight.Api.Aircrafts.DomainEventHandlers
{
    /* U Booking microservice imam DomainEventHandler koji se aktivira automatski jer je povezan na EventStore Subscriber 
      ali Flight microservice nije EventSourcing, pa ovaj handler se aktivira automatski zbog MediatR jer 
      FlightDbContext je nasledio ApplicationDbContextBase koji pomocu MediatR u CommitTransactionAsync dispatch domainevent. */

    public class AircraftCreatedDomainEventHandler : INotificationHandler<AircraftCreatedDomainEvent>
    {
        private readonly IMongoCollection<AircraftReadModel> _aircraftReadModelCollection;

        public AircraftCreatedDomainEventHandler(IMongoDatabase mongoDatabase)
        {
            _aircraftReadModelCollection = mongoDatabase.GetCollection<AircraftReadModel>("AircraftReadModels");
        }

        public async Task Handle(AircraftCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
        {   
            var aircraftReadModel = new AircraftReadModel
            {
                // Id popuni Mongo i zato nije required, jer meni ne treba to polje, a njemu treba 
                AircraftId = domainEvent.Id,
                Name = domainEvent.Name,
                Model = domainEvent.Model,
                ManufacturingYear = domainEvent.ManufacturingYear,
                IsDeleted = false,
            };

            await _aircraftReadModelCollection.InsertOneAsync(aircraftReadModel, null, cancellationToken);
        }
    }
}
