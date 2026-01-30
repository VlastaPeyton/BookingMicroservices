using Flight.Api.Flights.Events.DomainEvents;
using Flight.Api.Flights.Models;
using MediatR;
using MongoDB.Driver;

namespace Flight.Api.Flights.DomainEventHandlers
{
    /* U Booking microservice imam DomainEventHandler koji se aktivira automatski jer je povezan na EventStore Subscriber 
      ali Flight microservice nije EventSourcing, pa ovaj handler se aktivira automatski zbog MediatR jer 
      FlightDbContext je nasledio ApplicationDbContextBase koji pomocu MediatR u CommitTransactionAsync dispatch domainevent. */
    public class FlightCreatedDomainEventHandler : INotificationHandler<FlightCreatedDomainEvent>
    {
        private readonly IMongoCollection<FlightReadModel> _flightReadModelCollection;

        public FlightCreatedDomainEventHandler(IMongoDatabase mongoDatabase)
        {
            _flightReadModelCollection = mongoDatabase.GetCollection<FlightReadModel>("FlightReadModels");
        }

        public async Task Handle(FlightCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var flightReadModel = new FlightReadModel
            {
                // Id nije required, jer to Mono popuni sam, jer mu treba, a meni ne
                FlightId = domainEvent.FlightId,
                FlightNumber = domainEvent.FlightNumber,
                AircraftId = domainEvent.AircraftId,
                DepartureDate = domainEvent.DepartureDate,
                DepartureAirportId = domainEvent.DepartureAirportId,
                ArriveDate = domainEvent.ArriveDate,
                ArriveAirportId = domainEvent.ArriveAirportId,
                DurationMinutes = domainEvent.DurationMinutes,
                FlightDate = domainEvent.FlightDate,
                Status = domainEvent.Status,
                Price = domainEvent.Price,
                IsDeleted = false,
            };

            await _flightReadModelCollection.InsertOneAsync(flightReadModel, null, cancellationToken);
        }
    }
}