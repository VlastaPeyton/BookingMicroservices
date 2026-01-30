using Flight.Api.Airports.Events.DomainEvents;
using Flight.Api.Airports.Models;
using MediatR;
using MongoDB.Driver;

namespace Flight.Api.Airports.DomainEventHandlers
{
    /* U Booking microservice imam DomainEventHandler koji se aktivira automatski jer je povezan na EventStore Subscriber 
     ali Flight microservice nije EventSourcing, pa ovaj handler se aktivira automatski zbog MediatR jer 
     FlightDbContext je nasledio ApplicationDbContextBase koji pomocu MediatR u CommitTransactionAsync dispatch domainevent. */

    public class AirportCreatedDomainEventHandler : INotificationHandler<AirportCreatedDomainEvent>
    {
        private readonly IMongoCollection<AirportReadModel> _airportReadModelCollection;
        public AirportCreatedDomainEventHandler(IMongoDatabase mongoDatabase)
        {
            _airportReadModelCollection = mongoDatabase.GetCollection<AirportReadModel>("AirportReadModels");
        }

        public async Task Handle(AirportCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var airportReadModel = new AirportReadModel 
            {
                // Id nije required jer to Mongo popuni sam, jer mu to treba, a meni ne
                AirportId = domainEvent.Id,
                Name = domainEvent.Name,
                Address = domainEvent.Address,
                Code = domainEvent.Code,
                IsDeleted = false
            };

            await _airportReadModelCollection.InsertOneAsync(airportReadModel, null, cancellationToken);
        }
    }
}
