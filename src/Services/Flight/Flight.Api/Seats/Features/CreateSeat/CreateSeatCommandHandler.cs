using System.Windows.Input;
using BuildingBlocks.Core.CQRS;
using Flight.Api.Data;
using Flight.Api.Flights.ValueObjects;
using Flight.Api.Seats.Enums;
using Flight.Api.Seats.Exceptions;
using Flight.Api.Seats.Models;
using Flight.Api.Seats.ValueObjects;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flight.Api.Seats.Features.CreateSeat
{   
    public record CreateSeatCommand(Guid SeatId, 
                                    int SeatNumber, 
                                    SeatTypeEnum SeatType, 
                                    SeatClassEnum SeatClass, 
                                    Guid FlightId) : ICommand<CreateSeatResult>;
    public record CreateSeatResult(Guid Id);

    public class CreateSeatCommandValidator : AbstractValidator<CreateSeatCommand>
    {
        public CreateSeatCommandValidator()
        {
            RuleFor(x => x.SeatNumber).NotEmpty().WithMessage("SeatNumber is required");
            RuleFor(x => x.FlightId).NotEmpty().WithMessage("FlightId is required");
            RuleFor(x => x.SeatClass).Must(p => (p.GetType().IsEnum &&
                                                 p == SeatClassEnum.FirstClass) ||
                                                 p == SeatClassEnum.Business ||
                                                 p == SeatClassEnum.Economy)
                                    .WithMessage("Status must be FirstClass, Business or Economy");
        }
    }

    public class CreateSeatCommandHandler : ICommandHandler<CreateSeatCommand, CreateSeatResult>
    {   
        // DbContext mi treba jer je ovo DDD, a ne EventSource
        private readonly FlightDbContext _flightDbContext;

        public CreateSeatCommandHandler(FlightDbContext flightDbContext)
        {
            _flightDbContext = flightDbContext;
        }

        public async Task<CreateSeatResult> Handle(CreateSeatCommand command, CancellationToken cancellationToken)
        {
            var seat = _flightDbContext.Seats.SingleOrDefaultAsync(x => x.Id == command.SeatId, cancellationToken);
            if (seat is not null)
                throw new SeatAlreadyExistException();

            var newSeatEntity = Seat.Create(SeatId.Of(command.SeatId),
                                            SeatNumber.Of(command.SeatNumber),
                                            command.SeatType,
                                            command.SeatClass,
                                            FlightId.Of(command.FlightId));
            // Dodaje domain event u agregat 

            var seatEntity = (await _flightDbContext.Seats.AddAsync(newSeatEntity, cancellationToken)).Entity; // Samo u ChangeTracker upise promenu

            // Ovo pokrece iz ApplicationDbContextBase: SaveChanges + Commit + DispatchDomainEvents koje hvatam automatski u DomainEventHandler folderu
            await _flightDbContext.BeginTransactionAsync(cancellationToken);
            await _flightDbContext.CommitTransactionAsync(cancellationToken);

            return new CreateSeatResult(seatEntity.Id);
        }
    }
}
