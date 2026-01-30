using Booking.Api.Exceptions;
using Booking.Api.Models;
using Booking.Api.ValueObjects;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.EventStore.Repositories.EvetnStore;
using BuildingBlocks.UserProviders;
using Flight;
using FluentValidation;
using Passenger;

namespace Booking.Api.Features.CreateBooking
{   
    public record CreateBookingCommand(Guid BookingId,Guid PassengerId, Guid FlightId, string Description) : ICommand<CreateBookingResult>;
    public record CreateBookingResult(Guid Id);
    // Command, Result, Request i Response ne smeju imati ValueObject tipove jer su oni rezervisani za Domain

    public class CreateBookingValidator : AbstractValidator<CreateBookingCommand>
    {
        public CreateBookingValidator()
        {
            RuleFor(x => x.PassengerId).NotNull().WithMessage("PassengerId is required");
            RuleFor(x => x.FlightId).NotNull().WithMessage("FlightId is required");
        }
    }
    
    public class CreateBookingCommandHandler : ICommandHandler<CreateBookingCommand, CreateBookingResult>
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IEventStoreRepository<Booking.Api.Models.Booking, BookingId> _eventStoreRepository;
        
        /* Booking microservice je gRPC client samo, pa nema Service folder, ali u Program.cs definisano kako da pozove Flight gRCP Server + 
           u Booking microservice kopiram isti flight.proto kao u gRPC Server Flight microservicu. Isto vazi i za passenger.proto */
        private readonly FlightGrpcService.FlightGrpcServiceClient _flightGrpcServiceClient;
        private readonly PassengerGrpcService.PassengerGrpcServiceClient _passengerGrpcServiceClient;

        public CreateBookingCommandHandler(ICurrentUserProvider currentUserProvider,
                                           IEventStoreRepository<Booking.Api.Models.Booking, BookingId> eventStoreRepository,
                                           FlightGrpcService.FlightGrpcServiceClient flightGrpcServiceClient,
                                           PassengerGrpcService.PassengerGrpcServiceClient passengerGrpcServiceClient)
        {
            _currentUserProvider = currentUserProvider;
            _eventStoreRepository = eventStoreRepository;
            _flightGrpcServiceClient = flightGrpcServiceClient;
            _passengerGrpcServiceClient = passengerGrpcServiceClient;
        }
        
        public async Task<CreateBookingResult> Handle(CreateBookingCommand command, CancellationToken cancellationToken)
        {   
            // gRPC endpoints u C# se automatski generisu sinc + async, a ja biram async 

            var flight = await _flightGrpcServiceClient.GetByIdGrpcAsync(new Flight.GetByIdRequestGrpc { Id = command.FlightId.ToString() }, cancellationToken: cancellationToken);
            if (flight is null)
                throw new FlightNotFoundException();

            var passenger = await _passengerGrpcServiceClient.GetByIdGrpcAsync(new Passenger.GetPassengerByIdRequestGrpc { Id = command.PassengerId.ToString() }, 
                                                                               cancellationToken: cancellationToken);

            var emptySeat = (await _flightGrpcServiceClient.GetAvailableSeatsGrpcAsync(new GetAvailableSeatsRequestGrpc{ FlightId = command.FlightId.ToString() }, 
                                                                                       cancellationToken: cancellationToken).ResponseAsync).SeatDtos.FirstOrDefault();

            // Moraju zagrade, jer sync overload vraca AsyncUnaryCall<GetAvailableSeatsResult>, a ResponseAsync vraca Task<GetAvailableSeatsResult>

            var reservation = await _eventStoreRepository.GetStreamByIdAsync(command.BookingId, cancellationToken); // AggregateRootEventStore

            if (reservation is not null)
                throw new BookingAlreadyExistsException(); // Ako u EventStore postoji stream za reservation agregat, znaci da booking postoji

            string userId = _currentUserProvider.GetCurrentUserId();
            string correlationId = command.BookingId.ToString();

            var aggregate = Booking.Api.Models.Booking.Create(BookingId.Of(command.BookingId),
                                                              PassengerInfo.Of(passenger.PassengerDto.Name),
                                                              Trip.Of(flight.FlightDto.FlightNumber,
                                                                      new Guid(flight.FlightDto.AircraftId),
                                                                      new Guid(flight.FlightDto.DepartureAirportId),
                                                                      new Guid(flight.FlightDto.ArriveAirportId),
                                                                      flight.FlightDto.FlightDate.ToDateTime(),
                                                                      (decimal)flight.FlightDto.Price,
                                                                      command.Description,
                                                                      emptySeat.SeatNumber
                                                                      )
                                                              );

            // Sacuvaj uncommitedEvents iz aggregate u EventStore 
            await _eventStoreRepository.SaveUncommittedEventsAsync(aggregate, userId, correlationId, cancellationToken);

            // Dispatch domain events via mediatR AKO BAS TREBA tek nakon sto je sacuvao u EventStore i to ako treba neki in-process handler, ali ne treba jer EventSource koristim i Subscription+EventHandler tamo

            await _flightGrpcServiceClient.ReserveSeatGrpcAsync(new ReserveSeatRequestGrpc
            {
                FlightId = flight.FlightDto.Id,
                SeatNumber = emptySeat.SeatNumber,
            },
            cancellationToken: cancellationToken);

            return new CreateBookingResult(command.BookingId);
        }
    }
}