using BuildingBlocks.Core.Domain;
using Flight.Api.Aircrafts.ValueObjects;
using Flight.Api.Airports.ValueObjects;
using Flight.Api.Flights.Enums;
using Flight.Api.Flights.Events.DomainEvents;
using Flight.Api.Flights.ValueObjects;

namespace Flight.Api.Flights.Models
{   
    // Ovo nije EventSourcing, vec obican DDD
    public class Flight : AggregateRoot<FlightId>
    {
        public FlightNumber FlightNumber { get; private set; } = default!;
        public AircraftId AircraftId { get; private set; } = default!;
        public AirportId DepartureAirportId { get; private set; } = default!;
        public AirportId ArriveAirportId { get; private set; } = default!;
        public DurationMinutes DurationMinutes { get; private set; } = default!;
        public FlightStatusEnum Status { get; private set; }
        public Price Price { get; private set; } = default!;
        public ArriveDate ArriveDate { get; private set; } = default!;
        public DepartureDate DepartureDate { get; private set; } = default!;
        public FlightDate FlightDate { get; private set; } = default!;

        public static Flight Create(FlightId id, 
                                    FlightNumber flightNumber,
                                    AircraftId aircraftId,
                                    DepartureDate departureDate,
                                    AirportId departureAirportId,
                                    ArriveDate arriveDate,
                                    AirportId arriveAirportId,
                                    DurationMinutes durationMinutes,
                                    FlightDate flightDate,
                                    FlightStatusEnum status,
                                    Price price) 
        {
            // DDD bez EventSourcing, a ne zelim da Sql Db generise Id automatski, pa ga u kodu generisem ja u Command objektu

            // Flight microservice ne koristi EventSourcing i zato odma zadajem stanje, dok AddDomainEvents samo u listu doda event
            var flight = new Flight
            {
                Id = id,
                FlightNumber = flightNumber,
                AircraftId = aircraftId,
                DepartureAirportId = departureAirportId,
                ArriveAirportId = arriveAirportId,
                DurationMinutes = durationMinutes,
                Status = status,
                Price = price,
                ArriveDate = arriveDate,
                DepartureDate = departureDate,
                FlightDate = flightDate,
                // Audit polja prilikom create imaju default vrednosti, ali prilikom poziva FlightDbContext tj ApplicationDbContextBase Audit kolone se setuju kroz BeforeSaveChanges interceptor metodu
            };

            var domainEvent = new FlightCreatedDomainEvent(flight.Id,
                                                           flight.FlightNumber,
                                                           flight.AircraftId,
                                                           flight.DepartureDate,
                                                           flight.DepartureAirportId,
                                                           flight.ArriveDate,
                                                           flight.ArriveAirportId,
                                                           flight.DurationMinutes,
                                                           flight.FlightDate,
                                                           flight.Status,
                                                           flight.Price);
                                                           
            flight.AddDomainEvent(domainEvent);

            return flight;
        }

        public void Update(FlightId id, 
                           FlightNumber flightNumber,
                           AircraftId aircraftId,
                           DepartureDate departureDate, 
                           AirportId departureAirportId,
                           ArriveDate arriveDate,
                           AirportId arriveAirportId,
                           DurationMinutes durationMinutes,
                           FlightDate flightDate,
                           FlightStatusEnum status,
                           Price price, 
                           bool isDeleted = false)
        {   
            // Ne menjam rucno Id kada azuriram ! 
            FlightNumber = flightNumber; 
            AircraftId = aircraftId;
            DepartureDate = departureDate;
            DepartureAirportId = departureAirportId;
            ArriveDate = arriveDate;
            ArriveAirportId = arriveAirportId;
            DurationMinutes = durationMinutes;
            FlightDate = flightDate;
            Status = status;
            Price = price;
            IsDeleted = isDeleted;

            var domainEvent = new FlightUpdatedDomainEvent(id,
                                                           flightNumber,
                                                           aircraftId,
                                                           departureDate,
                                                           departureAirportId,
                                                           arriveDate,
                                                           arriveAirportId,
                                                           durationMinutes,
                                                           flightDate,
                                                           status,
                                                           price,
                                                           isDeleted);

            AddDomainEvent(domainEvent);
        }

        public void Delete(FlightId id,
                           FlightNumber flightNumber,
                           AircraftId aircraftId,
                           DepartureDate departureDate,
                           AirportId departureAirportId,
                           ArriveDate arriveDate,
                           AirportId arriveAirportId,
                           DurationMinutes durationMinutes,
                           FlightDate flightDate,
                           FlightStatusEnum status,
                           Price price,
                           bool isDeleted = true)
        {
            // Ne menjam rucno Id kada azuriram(softDelete ili hardDelete)
            FlightNumber = flightNumber;
            AircraftId = aircraftId;
            DepartureDate = departureDate;
            DepartureAirportId = departureAirportId;
            ArriveDate = arriveDate;
            ArriveAirportId = arriveAirportId;
            DurationMinutes = durationMinutes;
            FlightDate = flightDate;
            Status = status;
            Price = price;
            IsDeleted = isDeleted;

            var domainEvent = new FlightDeletedDomainEvent(id,
                                                           flightNumber,
                                                           aircraftId,
                                                           departureDate,
                                                           departureAirportId,
                                                           arriveDate,
                                                           arriveAirportId,
                                                           durationMinutes,
                                                           flightDate,
                                                           status,
                                                           price,
                                                           isDeleted);
            
            AddDomainEvent(domainEvent);
        }
    }
}
