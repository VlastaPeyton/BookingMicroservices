using MediatR;
using MongoDB.Driver;
using Passenger.Api.Events.DomainEvents;
using Passenger.Api.Models;

namespace Passenger.Api.EventHandlers.DomainEventHandlers
{   /* U Booking microservice imam DomainEventHandler koji se aktivira automatski jer je povezan na EventStore Subscriber
      ali Passenger microservice nije EventSourcing, pa ovaj handler se aktivira automatski zbog MediatR jer
      FlightDbContext je nasledio ApplicationDbContextBase koji pomocu MediatR u CommitTransactionAsync dispatch domainevent. */
    public class PassengerCreatedDomainEventHandler : INotificationHandler<PassengerCreatedDomainEvent>
    {
        private readonly IMongoCollection<PassengerReadModel> _passengerReadModelCollection;

        public PassengerCreatedDomainEventHandler(IMongoDatabase mongoDatabase)
        {
            _passengerReadModelCollection = mongoDatabase.GetCollection<PassengerReadModel>("PassengerReadModels");
        }

        public async Task Handle(PassengerCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var passengerReadModel = new PassengerReadModel
            {
                // Id nije required, jer to Mono popuni sam, jer mu treba, a meni ne
                PassengerId = domainEvent.PassengerId,
                PassportNumber = domainEvent.PassportNumber,
                Name = domainEvent.PassengerName,
                PassengerType = domainEvent.PassengerType,
                Age = domainEvent.Age,
                IsDeleted = false,
            };

            await _passengerReadModelCollection.InsertOneAsync(passengerReadModel, null, cancellationToken);

        }
    }
}