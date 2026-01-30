using BuildingBlocks.Core.CQRS;
using Flight.Api.Flights.Dtos;
using Flight.Api.Flights.Exceptions;
using Flight.Api.Flights.Models;
using MongoDB.Driver;

namespace Flight.Api.Flights.Features.GetAvailableFlights
{
    public record GetAvailableFlightsQuery : IQuery<GetAvailableFlightsResult>;
    public record GetAvailableFlightsResult(IEnumerable<FlightDto> FlightDtos);

    public class GetAvailableFlightsQueryHandler : IQueryHandler<GetAvailableFlightsQuery, GetAvailableFlightsResult>
    {
        // Query handler samo nad Mongo read modelima radi, jer je CQRS + nema DomainEventHandler za query nikad
        private readonly IMongoCollection<FlightReadModel> _flightReadModelCollection;

        public GetAvailableFlightsQueryHandler(IMongoDatabase mongoDatabase)
        {
            _flightReadModelCollection = mongoDatabase.GetCollection<FlightReadModel>("FlightReadModels");
        }

        public async Task<GetAvailableFlightsResult> Handle(GetAvailableFlightsQuery query, CancellationToken cancellationToken)
        {
            var flights = await _flightReadModelCollection.Find(f => !f.IsDeleted).ToListAsync(cancellationToken);

            if (!flights.Any())
                throw new FlightNotFountException();

            var flightDtos = flights.Select(f => new FlightDto(f.FlightId,
                                                              f.FlightNumber,
                                                              f.AircraftId,
                                                              f.ArriveDate,
                                                              f.ArriveAirportId,
                                                              f.DepartureDate,
                                                              f.DepartureAirportId,
                                                              f.DurationMinutes,
                                                              f.FlightDate,
                                                              f.Status, // Prevodi enum u string automatski, jer ima annotation u read modelu 
                                                              f.Price));

            return new GetAvailableFlightsResult(flightDtos);

        }
    }
}
