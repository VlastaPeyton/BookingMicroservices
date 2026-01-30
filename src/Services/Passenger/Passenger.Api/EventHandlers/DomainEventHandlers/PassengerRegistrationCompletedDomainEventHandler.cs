using MediatR;
using MongoDB.Driver;
using Passenger.Api.Events.DomainEvents;
using Passenger.Api.Models;

namespace Passenger.Api.EventHandlers.DomainEventHandlers
{  
    /* U Booking microservice imam DomainEventHandler koji se aktivira automatski jer je povezan na EventStore Subscriber
      ali Passenger microservice nije EventSourcing, pa ovaj handler se aktivira automatski zbog MediatR jer
      FlightDbContext je nasledio ApplicationDbContextBase koji pomocu MediatR u CommitTransactionAsync dispatch domainevent. */
    public class PassengerRegistrationCompletedDomainEventHandler : INotificationHandler<PassengerRegistrationCompletedDomainEvent>
    {
        private readonly IMongoCollection<PassengerReadModel> _passengerReadModelCollection;

        public PassengerRegistrationCompletedDomainEventHandler(IMongoDatabase mongoDatabase)
        {
            _passengerReadModelCollection = mongoDatabase.GetCollection<PassengerReadModel>("PassengerReadModels");
        }

        public async Task Handle(PassengerRegistrationCompletedDomainEvent domainEvent, CancellationToken cancellationToken)
        {   
            var filter = Builders<PassengerReadModel>.Filter.Eq(p => p.PassengerId, domainEvent.PassengerId);

            var update = Builders<PassengerReadModel>.Update
                                                     .Set(p => p.PassengerType, domainEvent.PassengerType)
                                                     .Set(p => p.PassportNumber, domainEvent.PassportNumber)
                                                     .Set(p => p.Age, domainEvent.Age)
                                                     .Set(p => p.Name, domainEvent.Name)
                                                     .Set(p => p.IsDeleted, false);

            await _passengerReadModelCollection.UpdateOneAsync(filter,
                                                               update,
                                                               new UpdateOptions { IsUpsert = false }, // Ne kreiraj novu ako ne postoji, vec samo update ako postoji
                                                               cancellationToken);
        }
    }
}
