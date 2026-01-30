using BuildingBlocks.Core.CQRS;
using Flight.Api.Flights.Models;
using Flight.Api.Seats.Dtos;
using Flight.Api.Seats.Exceptions;
using Flight.Api.Seats.Models;
using MongoDB.Driver;

namespace Flight.Api.Seats.Features.GetAvailableSeats
{   
    public record GetAvailableSeatsQuery(Guid FlightId) : IQuery<GetAvailableSeatsResult>;
    public record GetAvailableSeatsResult(IEnumerable<SeatDto> SeatDtos);

    public class GetAvailableSeatsQueryHandler : IQueryHandler<GetAvailableSeatsQuery, GetAvailableSeatsResult>
    {
        // Query handler samo nad Mongo read modelima radi, jer je CQRS + nema DomainEventHandler za query nikad
        private readonly IMongoCollection<SeatReadModel> _seatReadModelCollection;
        public GetAvailableSeatsQueryHandler(IMongoDatabase mongoDatabase)
        {
            _seatReadModelCollection = mongoDatabase.GetCollection<SeatReadModel>("SeatReadModels");
        }

        public async Task<GetAvailableSeatsResult> Handle(GetAvailableSeatsQuery query, CancellationToken cancellationToken)
        {
            var seats = await _seatReadModelCollection.Find(s => s.FlightId == query.FlightId &&
                                                                !s.IsDeleted)
                                                       .ToListAsync(cancellationToken);
            if (!seats.Any())
            {
                throw new AllSeatsFullException();
            }

            var seatDtos = seats.Select(s => new SeatDto(s.SeatId,
                                                         s.SeatNumber,
                                                         s.Type,  // Prevodi string iz Mongo u enum automatski, jer ima annotation u read modelu 
                                                         s.Class, // Prevodi string iz Mongo u enum automatski, jer ima annotation u read modelu 
                                                         s.FlightId));
          

            return new GetAvailableSeatsResult(seatDtos);
        }
    }
}
