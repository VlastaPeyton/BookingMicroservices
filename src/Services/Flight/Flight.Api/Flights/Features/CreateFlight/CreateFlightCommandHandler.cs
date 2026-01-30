using BuildingBlocks.Core.CQRS;
using Flight.Api.Aircrafts.ValueObjects;
using Flight.Api.Airports.ValueObjects;
using Flight.Api.Data;
using Flight.Api.Flights.Enums;
using Flight.Api.Flights.Exceptions;
using Flight.Api.Flights.ValueObjects;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flight.Api.Flights.Features.CreateFlight
{   
    public record CreateFlightCommand(Guid FlightId, 
                                      string FlightNumber, 
                                      Guid AircraftId,
                                      DateTime DepartureDate,
                                      Guid DepartureAirportId,
                                      DateTime ArriveDate, 
                                      Guid ArriveAirportId,
                                      decimal DurationMinutes, 
                                      DateTime FlightDate, 
                                      FlightStatusEnum Status,
                                      decimal Price) : ICommand<CreateFlightResult>;
    public record CreateFlightResult(Guid Id);


    public class CreateFlightCommandValidator : AbstractValidator<CreateFlightCommand>
    {
        public CreateFlightCommandValidator()
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
    public class CreateFlightCommandHandler : ICommandHandler<CreateFlightCommand, CreateFlightResult>
    {
        // DbContext mi treba jer je ovo DDD, a ne EventSource
        private readonly FlightDbContext _flightDbContext;

        public CreateFlightCommandHandler(FlightDbContext flightDbContext)
        {
            _flightDbContext = flightDbContext;
        }

        public async Task<CreateFlightResult> Handle(CreateFlightCommand command, CancellationToken cancellationToken)
        {
            var flight = await _flightDbContext.Flights.SingleOrDefaultAsync(f => f.Id == command.FlightId, cancellationToken);
            if (flight is null)
                throw new FlightAlreadyExistException();

            var newFlightEntity = Models.Flight.Create(FlightId.Of(command.FlightId),
                                                       FlightNumber.Of(command.FlightNumber),
                                                       AircraftId.Of(command.AircraftId),
                                                       DepartureDate.Of(command.DepartureDate),
                                                       AirportId.Of(command.DepartureAirportId),
                                                       ArriveDate.Of(command.ArriveDate),
                                                       AirportId.Of(command.ArriveAirportId),
                                                       DurationMinutes.Of(command.DurationMinutes),
                                                       FlightDate.Of(command.FlightDate),
                                                       command.Status,
                                                       Price.Of(command.Price));
            // Dodaje domainEvent u agregat 


            var flightEntity = (await _flightDbContext.Flights.AddAsync(newFlightEntity, cancellationToken)).Entity; // Samo u ChangeTracker doda

            // Ovo pokrece iz ApplicationDbContextBase: SaveChanges + Commit + DispatchDomainEvents koje hvatam automatski u DomainEventHandler folderu
            await _flightDbContext.BeginTransactionAsync(cancellationToken);
            await _flightDbContext.CommitTransactionAsync(cancellationToken);

            return new CreateFlightResult(flightEntity.Id);
        }
    }
}
