using Booking.Api.Events.DomainEvents;
using Booking.Api.ValueObjects;
using BuildingBlocks.Core.Events;
using BuildingBlocks.EventStore.Domain;

namespace Booking.Api.Models
{
    public class Booking : AggregateRootEventSource<BookingId>
    {   
        public Trip Trip { get; private set; }
        public PassengerInfo PassengerInfo { get; private set; }

        public static Booking Create(BookingId id, PassengerInfo passengerInfo, Trip trip)
        {   // Id prosledjujem iz FE zbog EventStore i zato ga ovde ima u argumentima

            var booking = new Booking(); // Prazan, jer menjam stanje samo kroz AddDomainEvent, zato sto Booking microservice koristi EventSourcing

            var domainEvent = new BookingCreatedDomainEvent(id, passengerInfo, trip); // DomainEvent treba imati ValueObject tipove

            booking.AddDomainEvent(domainEvent); // Dodaje domainEvent u listu + menja stanje agregata u memoriji 

            return booking;
        }

        // Koristim kada citam iz EventStoreDb u LoadFromHistory base metodi
        public void When(IDomainEvent domainEvent)
        {
            if (domainEvent is BookingCreatedDomainEvent e)
            {
                Apply(e);
            }
        }

        // Menja stanje agregata u memoriji i nikad ne sme menjati Version
        private void Apply(BookingCreatedDomainEvent domainEvent)
        {
            Id = domainEvent.Id; // Jer ima implicit cast u BookingId
            Trip = domainEvent.Trip;
            PassengerInfo = domainEvent.PassengerInfo;
        }
    }
}
