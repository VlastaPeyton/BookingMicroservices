using BuildingBlocks.Core.CQRS;
using Flight.Api.Flights.Dtos;
using Flight.Api.Flights.Exceptions;
using Flight.Api.Flights.Models;
using MongoDB.Driver;

namespace Flight.Api.Flights.Features.GetFlightById
{
    public record GetFlightByIdQuery(Guid FlightId) : IQuery<GetFlightByIdResult>;
    public record GetFlightByIdResult(FlightDto FlightDto);

    public class GetFlightByIdQueryHandler : IQueryHandler<GetFlightByIdQuery, GetFlightByIdResult>
    {   
        // Query handler samo nad Mongo read modelima radi, jer je CQRS + nema DomainEventHandler za query nikad
        private readonly IMongoCollection<FlightReadModel> _flightReadModelCollection;

        public GetFlightByIdQueryHandler(IMongoDatabase mongoDatabase)
        {
            _flightReadModelCollection = mongoDatabase.GetCollection<FlightReadModel>("FlightReadModels");
        }

        public async Task<GetFlightByIdResult> Handle(GetFlightByIdQuery query, CancellationToken cancellationToken)
        {
            var flight = await _flightReadModelCollection.Find(f => f.FlightId == query.FlightId && !f.IsDeleted)
                                                         .FirstOrDefaultAsync(cancellationToken);

            if (flight is null)
                throw new FlightNotFountException();

            var flightDto = new FlightDto(flight.FlightId,
                                          flight.FlightNumber,
                                          flight.AircraftId,
                                          flight.ArriveDate,
                                          flight.ArriveAirportId,
                                          flight.DepartureDate,
                                          flight.DepartureAirportId,
                                          flight.DurationMinutes,
                                          flight.FlightDate,
                                          flight.Status, // Prevodi string iz Mongo u enum automatski, jer ima annotation u read modelu 
                                          flight.Price);
                                    
            return new GetFlightByIdResult(flightDto);
        }
    }
}
