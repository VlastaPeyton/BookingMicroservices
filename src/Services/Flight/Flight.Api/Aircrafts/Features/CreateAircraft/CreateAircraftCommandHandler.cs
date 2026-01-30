using BuildingBlocks.Core.CQRS;
using Flight.Api.Aircrafts.Exceptions;
using Flight.Api.Aircrafts.Models;
using Flight.Api.Aircrafts.ValueObjects;
using Flight.Api.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flight.Api.Aircrafts.Features.CreateAircraft
{   
    public record CreateAircraftCommand(Guid AircraftId, string Name, string Model, int ManufacturingYear) : ICommand<CreateAircraftResult>;
    public record CreateAircraftResult(Guid Id);
    // Command, Result, Request i Response ne smeju imati ValueObject tipove jer su oni rezervisani za Domain


    public class CreateAircraftCommandValidator : AbstractValidator<CreateAircraftCommand>
    {
        public CreateAircraftCommandValidator()
        {
            RuleFor(x => x.Model).NotEmpty().WithMessage("Model is required");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.ManufacturingYear).NotEmpty().WithMessage("ManufacturingYear is required");
        }
    }

    public class CreateAircraftCommandHandler : ICommandHandler<CreateAircraftCommand, CreateAircraftResult>
    {
        // DbContext mi treba jer je ovo DDD, a ne EventSource
        private readonly FlightDbContext _flightDbContext;

        public CreateAircraftCommandHandler(FlightDbContext flightDbContext, IMediator mediator)
        {
            _flightDbContext = flightDbContext;
        }

        public async Task<CreateAircraftResult> Handle(CreateAircraftCommand command, CancellationToken cancellationToken)
        {
            var aircraft = await _flightDbContext.Aircrafts.SingleOrDefaultAsync(a => a.Model.Value == command.Model, cancellationToken);

            if (aircraft is not null)
                throw new AircraftAlreadyExistException();

            var newAircraftEntity = Aircraft.Create(AircraftId.Of(command.AircraftId),
                                                    Name.Of(command.Name),
                                                    Model.Of(command.Model),
                                                    ManufacturingYear.Of(command.ManufacturingYear));
            // Dodaje domainEvent u agregat 

            var newAircraft = (await _flightDbContext.Aircrafts.AddAsync(newAircraftEntity, cancellationToken)).Entity; // Doda u ChangeTracker samo 

            // Ovo pokrece iz ApplicationDbContextBase: SaveChanges + Commit + DispatchDomainEvents koje hvatam automatski u DomainEventHandler folderu
            await _flightDbContext.BeginTransactionAsync(cancellationToken);
            await _flightDbContext.CommitTransactionAsync(cancellationToken);

            return new CreateAircraftResult(newAircraft.Id);
        }
    }
}
