using BuildingBlocks.Core.CQRS;
using MongoDB.Driver;
using Passenger.Api.Dtos;
using Passenger.Api.Exception;
using Passenger.Api.Models;

namespace Passenger.Api.Features.GetPassengerById
{
    // GetPassengerByIdQueryHandler pozivam iz PassengerGrpcService, ne iz Endpoint
    public record GetPassengerByIdQuery(Guid PassengerId) : IQuery<GetPassengerByIdResult>;
    public record GetPassengerByIdResult(PassengerDto PassengerDto);
    public class GetPassengerByIdQueryHandler : IQueryHandler<GetPassengerByIdQuery, GetPassengerByIdResult>
    {
        // Query handler samo nad Mongo read modelima radi, jer je CQRS + nema DomainEventHandler za query nikad
        private readonly IMongoCollection<PassengerReadModel> _passengerReadModelCollection;

        public GetPassengerByIdQueryHandler(IMongoDatabase mongoDatabase)
        {
            _passengerReadModelCollection = mongoDatabase.GetCollection<PassengerReadModel>("PassengerReadModels");
        }

        public async Task<GetPassengerByIdResult> Handle(GetPassengerByIdQuery query, CancellationToken cancellationToken)
        {
            var passenger = await _passengerReadModelCollection.Find(p => p.PassengerId == query.PassengerId && !p.IsDeleted)
                                                               .FirstOrDefaultAsync(cancellationToken);

            if (passenger is null)
                throw new PassengerNotFoundException();

            var passengerDto = new PassengerDto(passenger.PassengerId,
                                                passenger.Name,
                                                passenger.PassportNumber,
                                                passenger.PassengerType, // Prevodi string iz Mongo u enum automatski, jer ima annotation u read modelu 
                                                passenger.Age);

            return new GetPassengerByIdResult(passengerDto);
        }
    }
}
