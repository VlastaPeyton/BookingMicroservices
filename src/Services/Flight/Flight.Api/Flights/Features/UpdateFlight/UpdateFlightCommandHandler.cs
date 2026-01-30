using BuildingBlocks.Core.CQRS;
using Flight.Api.Aircrafts.ValueObjects;
using Flight.Api.Airports.ValueObjects;
using Flight.Api.Data;
using Flight.Api.Flights.Enums;
using Flight.Api.Flights.Exceptions;
using Flight.Api.Flights.ValueObjects;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Flight.Api.Flights.Features.UpdateFlight
{   
    public record UpdateFlightCommand(Guid FlightId, 
                                     string FlightNumber, 
                                     Guid AircraftId,
                                     DateTime DepartureDate,
                                     Guid DepartureAirportId,
                                     DateTime ArriveDate, 
                                     Guid ArriveAirportId, 
                                     decimal DurationMinutes, 
                                     DateTime FlightDate,
                                     FlightStatusEnum Status,
                                     decimal Price,
                                     bool IsDeleted) : ICommand<UpdateFlightResult>;

    public record UpdateFlightResult(Guid Id);

    public class UpdateFlightCommandValidator : AbstractValidator<UpdateFlightCommand>
    {
        public UpdateFlightCommandValidator()
        {
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0");

            RuleFor(x => x.Status).Must(p => (p.GetType().IsEnum &&
                                              p == FlightStatusEnum.Flying) ||
                                              p == FlightStatusEnum.Canceled ||
                                              p == FlightStatusEnum.Delay ||
                                              p == FlightStatusEnum.Completed)
                .WithMessage("Status must be Flying, Delay, Canceled or Completed");

            RuleFor(x => x.AircraftId).NotEmpty().WithMessage("AircraftId must be not empty");
            RuleFor(x => x.DepartureAirportId).NotEmpty().WithMessage("DepartureAirportId must be not empty");
            RuleFor(x => x.ArriveAirportId).NotEmpty().WithMessage("ArriveAirportId must be not empty");
            RuleFor(x => x.DurationMinutes).GreaterThan(0).WithMessage("DurationMinutes must be greater than 0");
            RuleFor(x => x.FlightDate).NotEmpty().WithMessage("FlightDate must be not empty");
        }
    }

    public class UpdateFlightCommandHandler : ICommandHandler<UpdateFlightCommand, UpdateFlightResult>
    {
        // DbContext mi treba jer je ovo DDD, a ne EventSource
        private readonly FlightDbContext _flightDbContext;

        public UpdateFlightCommandHandler(FlightDbContext flightDbContext)
        {
            _flightDbContext = flightDbContext;
        }

        public async Task<UpdateFlightResult> Handle(UpdateFlightCommand command, CancellationToken cancellationToken)
        {
            var flight = await _flightDbContext.Flights.SingleOrDefaultAsync(x => x.Id == command.FlightId, cancellationToken);
            if (flight is null)
            {
                throw new FlightNotFountException();
            }

            flight.Update(FlightId.Of(command.FlightId), 
                          FlightNumber.Of(command.FlightNumber), 
                          AircraftId.Of(command.AircraftId),
                          DepartureDate.Of(command.DepartureDate),
                          AirportId.Of(command.DepartureAirportId),
                          ArriveDate.Of(command.ArriveDate), 
                          AirportId.Of(command.ArriveAirportId), 
                          DurationMinutes.Of(command.DurationMinutes), 
                          FlightDate.Of(command.FlightDate), 
                          command.Status,
                          Price.Of(command.Price), 
                          command.IsDeleted);
            // Dodaje domainEvent u agregat 

            var updateFlight = _flightDbContext.Flights.Update(flight).Entity; // Azurira ChangeTracker samo

            // Ovo pokrece iz ApplicationDbContextBase: SaveChanges + Commit + DispatchDomainEvents koje hvatam automatski u DomainEventHandler folderu
            await _flightDbContext.BeginTransactionAsync(cancellationToken);
            await _flightDbContext.CommitTransactionAsync(cancellationToken);

            return new UpdateFlightResult(flight.Id);
        }
    }
}
