using BuildingBlocks.Core.CQRS;
using Flight.Api.Airports.Exceptions;
using Flight.Api.Airports.Models;
using Flight.Api.Airports.ValueObjects;
using Flight.Api.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Flight.Api.Airports.Features.CreateAirport
{   
    public record CreateAirportCommand(Guid AirportId, string Name, string Address, string Code) : ICommand<CreateAirportResult>;
    public record CreateAirportResult(Guid Id);
    // Command, Result, Request i Response ne smeju imati ValueObject tipove jer su oni rezervisani za Domain

    public class CreateAirportCommandValidator : AbstractValidator<CreateAirportCommand>
    {
        public CreateAirportCommandValidator()
        {
            RuleFor(x => x.Code).NotEmpty().WithMessage("Code is required");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required");
        }
    }

    public class CreateAirportCommandHandler : ICommandHandler<CreateAirportCommand, CreateAirportResult>
    {   
        // DbContext mi treba jer je ovo DDD, a ne EventSource
        private readonly FlightDbContext _flightDbContext;

        public CreateAirportCommandHandler(FlightDbContext flightDbContext)
        {
            _flightDbContext = flightDbContext;
        }

        public async Task<CreateAirportResult> Handle(CreateAirportCommand command, CancellationToken cancellationToken)
        {
            var airport = await _flightDbContext.Airports.SingleOrDefaultAsync(a => a.Code.Value == command.Code, cancellationToken);
            if (airport is not null)
                throw new AirportAlreadyExistException();

            var newAirportEntity = Airport.Create(AirportId.Of(command.AirportId),
                                                  Name.Of(command.Name),
                                                  Address.Of(command.Address),
                                                  Code.Of(command.Code));
            // Dodaje domainEvent u agregat 

            var newAirport = (await _flightDbContext.Airports.AddAsync(newAirportEntity, cancellationToken)).Entity; // Doda u ChangeTracker samo

            // Ovo pokrece iz ApplicationDbContextBase: SaveChanges + Commit + DispatchDomainEvents koje hvatam automatski u DomainEventHandler folderu
            await _flightDbContext.BeginTransactionAsync(cancellationToken);
            await _flightDbContext.CommitTransactionAsync(cancellationToken);

            return new CreateAirportResult(newAirport.Id);
        }
    }
}
