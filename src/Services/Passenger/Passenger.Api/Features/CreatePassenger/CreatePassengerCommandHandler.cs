using BuildingBlocks.Core.CQRS;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Passenger.Api.Data;
using Passenger.Api.Enums;
using Passenger.Api.ValueObjects;

namespace Passenger.Api.Features.CreatePassenger
{
    // Nema CreatePassengerEndpoint, jer CreatePassengerCommandHandler se poziva iz PassengerCreatedIntegrationEventHandler 
    public record CreatePassengerCommand(Guid PassengerId, 
                                         string PassengerName, 
                                         string PassportNumber, 
                                         int PassengerAge,
                                         PassengerTypeEnum PassengerType) : ICommand<Unit>;

    public class CreatePassengerCommandValidator : AbstractValidator<CreatePassengerCommand>
    {
        public CreatePassengerCommandValidator()
        {
            RuleFor(x => x.PassengerId).NotEmpty().WithMessage("PassengerId must not be empty");
            RuleFor(x => x.PassengerName).NotEmpty().WithMessage("PassengerName must not be empty");
            RuleFor(x => x.PassportNumber).NotEmpty().WithMessage("PassportNumber must not be empty");
        }
    }

    public class CreatePassengerCommandHandler : ICommandHandler<CreatePassengerCommand, Unit>
    {
        private readonly PassengerDbContext _passengerDbContext;
        private readonly ISender _sender;

        public CreatePassengerCommandHandler(PassengerDbContext passengerDbContext, ISender sender)
        {
            _passengerDbContext = passengerDbContext;
            _sender = sender;
        }

        public async Task<Unit> Handle(CreatePassengerCommand command, CancellationToken cancellationToken)
        {
            var passengerExist = await _passengerDbContext.Passengers.AnyAsync(x => x.PassportNumber == command.PassportNumber, cancellationToken);
            if (passengerExist)
                return Unit.Value;

            var passenger = Passenger.Api.Models.Passenger.Create(PassengerId.Of(command.PassengerId),
                                                                  Name.Of(command.PassengerName),
                                                                  PassportNumber.Of(command.PassportNumber),
                                                                  Age.Of(command.PassengerAge),
                                                                  command.PassengerType);
            // DomainEvent added to agregate

            await _passengerDbContext.AddAsync(passenger, cancellationToken); // Samo u ChangeTracker za sada, jer ceka SaveChanges za upis u bazu

            // Ovo pokrece iz ApplicationDbContextBase: SaveChanges + Commit + DispatchDomainEvents koje hvatam automatski u DomainEventHandler folderu
            await _passengerDbContext.BeginTransactionAsync(cancellationToken);
            await _passengerDbContext.CommitTransactionAsync(cancellationToken);

            return Unit.Value;
        }
    }
}