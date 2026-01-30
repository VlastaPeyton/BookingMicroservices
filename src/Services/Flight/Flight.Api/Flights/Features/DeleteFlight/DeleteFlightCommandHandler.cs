using BuildingBlocks.Core.CQRS;
using Flight.Api.Data;
using Flight.Api.Flights.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flight.Api.Flights.Features.DeleteFlight
{   
    public record DeleteFlightCommand(Guid FlightId) : ICommand;

    public class DeleteFlightCommandValidator : AbstractValidator<DeleteFlightCommand>
    {
        public DeleteFlightCommandValidator()
        {
            RuleFor(x => x.FlightId).NotEmpty();
        }
    }

    public class DeleteFlightCommandHandler : ICommandHandler<DeleteFlightCommand, Unit>
    {
        // DbContext mi treba jer je ovo DDD, a ne EventSource
        private readonly FlightDbContext _flightDbContext;

        public DeleteFlightCommandHandler(FlightDbContext flightDbContext)
        {
            _flightDbContext = flightDbContext;
        }

        public async Task<Unit> Handle(DeleteFlightCommand command, CancellationToken cancellationToken)
        {
            var flight = await _flightDbContext.Flights.SingleOrDefaultAsync(x => x.Id == command.FlightId, cancellationToken);
            if (flight is null)
                throw new FlightNotFountException();

            flight.Delete(flight.Id,
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
            // Dodaje domainEvent u agregat 

            var deleteFlight = _flightDbContext.Flights.Update(flight).Entity; // U ChangeTracker samo azurira 

            // Ovo pokrece iz ApplicationDbContextBase: SaveChanges + Commit + DispatchDomainEvents koje hvatam automatski u DomainEventHandler folderu
            await _flightDbContext.BeginTransactionAsync(cancellationToken);
            await _flightDbContext.CommitTransactionAsync(cancellationToken);

            return Unit.Value;
            
        }
    }
}
