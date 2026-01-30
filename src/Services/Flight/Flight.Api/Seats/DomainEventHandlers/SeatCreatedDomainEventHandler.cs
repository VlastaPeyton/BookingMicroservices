using System.Runtime.ConstrainedExecution;
using BuildingBlocks.Core.Events;
using BuildingBlocks.EfCore;
using Flight.Api.Flights.Models;
using Flight.Api.Seats.Events.DomainEvents;
using Flight.Api.Seats.Models;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using MongoDB.Driver;

namespace Flight.Api.Seats.DomainEventHandlers
{
    /* U Booking microservice imam DomainEventHandler koji se aktivira automatski jer je povezan na EventStore Subscriber
      ali Flight microservice nije EventSourcing, pa ovaj handler se aktivira automatski zbog MediatR jer
      FlightDbContext je nasledio ApplicationDbContextBase koji pomocu MediatR u CommitTransactionAsync dispatch domainevent. */
    public class SeatCreatedDomainEventHandler : INotificationHandler<SeatCreatedDomainEvent>
    {
        private readonly IMongoCollection<SeatReadModel> _seatReadModelCollection;

        public SeatCreatedDomainEventHandler(IMongoDatabase mongoDatabase)
        {
            _seatReadModelCollection = mongoDatabase.GetCollection<SeatReadModel>("SeatReadModels");
        }

        public async Task Handle(SeatCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var seatReadModel = new SeatReadModel
            {
                // Id nije required, jer to Mono popuni sam, jer mu treba, a meni ne
                SeatId = domainEvent.SeatId,
                SeatNumber = domainEvent.SeatNumber,
                Type = domainEvent.Type,
                Class = domainEvent.Class,
                FlightId = domainEvent.FlightId,
                IsDeleted = false
            };

            await _seatReadModelCollection.InsertOneAsync(seatReadModel, null, cancellationToken);
        }
    }
}
