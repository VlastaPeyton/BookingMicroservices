using BuildingBlocks.Core.Domain;
using Flight.Api.Airports.Events.DomainEvents;
using Flight.Api.Airports.ValueObjects;

namespace Flight.Api.Airports.Models
{   
    // Obican DDD , ne EventSourcing
    public class Airport : AggregateRoot<AirportId>
    {
        public Name Name { get; private set; } = default!;
        public Address Address { get; private set; } = default!;
        public Code Code { get; private set; } = default!;

        public static Airport Create(AirportId id, Name name, Address address, Code code)
        {
            // DDD bez EventSourcing, a ne zelim da Sql Db generise Id automatski, pa ga u kodu generisem ja u Command objektu

            // Flight microservice ne koristi EventSourcing i zato odma zadajem stanje, dok AddDomainEvents samo u listu doda event
            var airport = new Airport
            {
                Id = id,
                Name = name,
                Address = address,
                Code = code,
                // Audit polja prilikom create imaju default vrednosti, ali prilikom poziva FlightDbContext tj ApplicationDbContextBase Audit kolone se setuju kroz BeforeSaveChanges metodu
            };

            var domainEvent = new AirportCreatedDomainEvent(airport.Id,
                                                            airport.Name,
                                                            airport.Address,
                                                            airport.Code);
                                                           
            airport.AddDomainEvent(domainEvent);

            return airport;
        }
    }
}
