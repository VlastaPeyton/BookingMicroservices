using BuildingBlocks.Core.CQRS;
using Flight.Api.Data;
using Flight.Api.Seats.Exceptions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Flight.Api.Seats.Features.ReserveSeat
{   
    public record ReserveSeatCommand(Guid FlightId, int SeatNumber) : ICommand<ReserveSeatResult>;
    public record ReserveSeatResult(int SeatNumber); 

    public class ReserveSeatCommandValidator : AbstractValidator<ReserveSeatCommand>
    {
        public ReserveSeatCommandValidator()
        {
            RuleFor(x => x.FlightId).NotEmpty().WithMessage("FlightId must not be empty");
            RuleFor(x => x.SeatNumber).NotEmpty().WithMessage("SeatNumber must not be empty");
        }
    }

    public class ReserveSeatCommandHandler : ICommandHandler<ReserveSeatCommand, ReserveSeatResult>
    {
        // DbContext mi treba jer je ovo DDD, a ne EventSource
        private readonly FlightDbContext _flightDbContext;
        public ReserveSeatCommandHandler(FlightDbContext flightDbContext)
        {
            _flightDbContext = flightDbContext;
        }

        public async Task<ReserveSeatResult> Handle(ReserveSeatCommand command, CancellationToken cancellationToken)
        {
            var seat = await _flightDbContext.Seats.SingleOrDefaultAsync(x => x.SeatNumber.Value == command.SeatNumber &&
                                                                        x.FlightId == command.FlightId, 
                                                                        cancellationToken);

            if (seat is null)
                throw new SeatNumberIncorrectException();

            seat.ReserveSeat();
            // Domain event ubacio u agregat 

            var updatedSeat = _flightDbContext.Seats.Update(seat).Entity; // Samo u ChangeTracker unese izmene za sada 

            // Ovo pokrece iz ApplicationDbContextBase: SaveChanges + Commit + DispatchDomainEvents koje hvatam automatski u DomainEventHandler folderu
            await _flightDbContext.BeginTransactionAsync(cancellationToken);
            await _flightDbContext.CommitTransactionAsync(cancellationToken);

            return new ReserveSeatResult(updatedSeat.SeatNumber);
        }
    }
}
