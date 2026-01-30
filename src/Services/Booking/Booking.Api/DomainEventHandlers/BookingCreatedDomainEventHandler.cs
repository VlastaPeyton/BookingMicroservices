using Booking.Api.Events.DomainEvents;
using Booking.Api.Mapper;
using Booking.Api.Models;
using BuildingBlocks.EventStore.Subscriptions;
using MongoDB.Driver;

namespace Booking.Api.DomainEventHandlers
{
    // SubscriptionToEventStore background worker reaguje na svaki novi domain event u EventStore i automatski poziva svaku CustomDomainEventHandler
    public class BookingCreatedDomainEventHandler : DomainEventHandlerES<BookingCreatedDomainEvent>
    {
        private readonly IMongoCollection<BookingReadModel> _bookingReadModelCollection; 

        public BookingCreatedDomainEventHandler(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
            _bookingReadModelCollection = mongoDatabase.GetCollection<BookingReadModel>("BookingReadModels");
        }

        protected override async Task HandleEventAsync(BookingCreatedDomainEvent domainEvent, CancellationToken ct)
        {   
            // Pomocu implicit operator u ValueObject, polja iz BookingCreatedDomainEvent se mapiraju u primitive types of ReadModel 

            var bookingReadModel = new BookingReadModel
            {
                // Id nije required, pa ga Mongo sam popuni, jer meni to polje ne treba 
                BookingId = domainEvent.Id,
                Trip = domainEvent.Trip.FromTripToTripReadModelDto(),
                PassengerInfo = domainEvent.PassengerInfo,
                IsDeleted = false
            };

            await _bookingReadModelCollection.InsertOneAsync(bookingReadModel, null, ct);
        }
    }
}
