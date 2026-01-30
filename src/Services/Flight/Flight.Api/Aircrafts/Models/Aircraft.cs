using BuildingBlocks.Core.Domain;
using Flight.Api.Aircrafts.Events.DomainEvents;
using Flight.Api.Aircrafts.ValueObjects;

namespace Flight.Api.Aircrafts.Models
{
    // Obican DDD , ne EventSourcing
    public class Aircraft : AggregateRoot<AircraftId>
    {
        public Name Name { get; private set; } = default!;
        public Model Model { get; private set; } = default!;
        public ManufacturingYear ManufacturingYear { get; private set; } = default!;

        public static Aircraft Create(AircraftId id, Name name, Model model, ManufacturingYear manufacturingYear)
        {
            // DDD bez EventSourcing, a ne zelim da Sql Db generise Id automatski, pa ga u kodu generisem ja u Command objektu

            // Flight microservice ne koristi EventSourcing i zato odma zadajem stanje, dok AddDomainEvents samo u listu doda event
            var aircraft = new Aircraft
            {
                Id = id,
                Name = name,
                Model = model,
                ManufacturingYear = manufacturingYear
                // Audit polja prilikom create imaju default vrednosti, ali prilikom poziva FlightDbContext tj ApplicationDbContextBase Audit kolone se setuju kroz BeforeSaveChanges metodu
            };

            var domainEvent = new AircraftCreatedDomainEvent(aircraft.Id,
                                                             aircraft.Name,
                                                             aircraft.Model,
                                                             aircraft.ManufacturingYear);
                                                             

            aircraft.AddDomainEvent(domainEvent);

            return aircraft;
        }
    }
}
