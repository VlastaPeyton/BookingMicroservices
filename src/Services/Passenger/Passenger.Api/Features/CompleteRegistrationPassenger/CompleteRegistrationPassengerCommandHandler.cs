using BuildingBlocks.Core.CQRS;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Passenger.Api.Data;
using Passenger.Api.Dtos;
using Passenger.Api.Enums;
using Passenger.Api.Exception;
using Passenger.Api.ValueObjects;

namespace Passenger.Api.Features.CompleteRegistrationPassenger
{   
    public record CompleteRegistrationPassengerCommand(Guid PassengerId,
                                                       string PassportNumber, 
                                                       PassengerTypeEnum PassengerType, 
                                                       int Age) : ICommand<CompleteRegistrationPassengerResult>;
    public record CompleteRegistrationPassengerResult(PassengerDto PassengerDto);

    public class CompleteRegistrationPassengerCommandValidator : AbstractValidator<CompleteRegistrationPassengerCommand>
    {
        public CompleteRegistrationPassengerCommandValidator()
        {
            RuleFor(x => x.PassportNumber).NotNull().WithMessage("The PassportNumber is required!");
            RuleFor(x => x.Age).GreaterThan(0).WithMessage("The Age must be greater than 0!");
            RuleFor(x => x.PassengerType).Must(p => p.GetType().IsEnum &&
                                                    p == PassengerTypeEnum.Baby ||
                                                    p == PassengerTypeEnum.Female ||
                                                    p == PassengerTypeEnum.Male ||
                                                    p == PassengerTypeEnum.Unknown)
                                        .WithMessage("PassengerType must be Male, Female, Baby or Unknown");
        }
    }

    public class CompleteRegistrationPassengerCommandHandler : ICommandHandler<CompleteRegistrationPassengerCommand, CompleteRegistrationPassengerResult>
    {
        // DbContext mi treba jer je ovo DDD, a ne EventSource
        private readonly PassengerDbContext _passengerDbContext;

        public CompleteRegistrationPassengerCommandHandler(PassengerDbContext passengerDbContext)
        {
            _passengerDbContext = passengerDbContext;
        }

        public async Task<CompleteRegistrationPassengerResult> Handle(CompleteRegistrationPassengerCommand command, CancellationToken cancellationToken)
        {
            var passenger = await _passengerDbContext.Passengers.SingleOrDefaultAsync(x => x.PassportNumber.Value == command.PassportNumber, cancellationToken);
            if (passenger is null)
                throw new PassengerNotExistException();

            passenger.CompleteRegistrationPassenger(passenger.Id,
                                                    passenger.Name,
                                                    passenger.PassportNumber,
                                                    passenger.PassengerType, // u bazi su vec ValueObjects i zato ne mora ovako 
                                                    passenger.Age);
                                                    
            // DomainEvent added to agregate

            var updatePassenger = _passengerDbContext.Passengers.Update(passenger).Entity; // Samo u ChangeTracker za sada unete promene i ceka se SaveChanges

            var passengerDto = new PassengerDto(updatePassenger.Id, 
                                                updatePassenger.Name,
                                                updatePassenger.PassportNumber,
                                                updatePassenger.PassengerType,
                                                updatePassenger.Age);

            // Ovo pokrece iz ApplicationDbContextBase: SaveChanges + Commit + DispatchDomainEvents koje hvatam automatski u DomainEventHandler folderu
            await _passengerDbContext.BeginTransactionAsync(cancellationToken);
            await _passengerDbContext.CommitTransactionAsync(cancellationToken);

            return new CompleteRegistrationPassengerResult(passengerDto);
        }
    }
}
