using System.Collections.Generic;
using Flight.Api.Flights.Enums;
using Flight.Api.Flights.Features.GetFlightById;
using Flight.Api.Flights.Mappers;
using Flight.Api.Seats.Dtos;
using Flight.Api.Seats.Features.GetAvailableSeats;
using Flight.Api.Seats.Features.ReserveSeat;
using Flight.Api.Seats.Mappers;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;

namespace Flight.Api.GrpcServer.Services
{
    /* Fligh microservice je gRCP Both/Server, pa moram ovde definisati gRPC endpoints iz flight.proto koje Booking poziva
       Flight. - jer flight.proto ima package fligt, pa namespace konflikt sa Flight.Api imenom projekta 
       Nije pozeljno da ime klase bude isto kao ime service u .proto jer C# napravi static class iz .proto a ovo nije static. */
    public class FlightGrpcEndpoints : FlightGrpcService.FlightGrpcServiceBase
    {
        private readonly ISender _sender; // MediatR

        public FlightGrpcEndpoints(ISender sender)
        {
            _sender = sender;
        }

        public override async Task<GetFlightByIdResultGrpc> GetByIdGrpc(GetByIdRequestGrpc request, ServerCallContext context)
        {
            var result = await _sender.Send(new GetFlightByIdQuery(new Guid(request.Id)), context.CancellationToken); // = GetFlightByIdResult(FlightDto FlightDto);

            // Mapiram iz GetFlightByIdResult (C#) u GetFlightByIdResultGrpc (flight.proto) tj iz FlightDto (C#) u FlightResponse (flight.proto)
            var gRpcResponse = new GetFlightByIdResultGrpc
            {   // Zbog gRPC, moram navesti ime polja bez obzira na tip polja
                FlightDto = new FlightResponse
                {
                    Id = result.FlightDto.FlightId.ToString(),
                    FlightNumber = result.FlightDto.FlightNumber,
                    AircraftId = result.FlightDto.AircraftId.ToString(),
                    DepartureAirportId = result.FlightDto.DepartureAirportId.ToString(),
                    DepartureDate = Timestamp.FromDateTime(result.FlightDto.DepartureDate), // TimeStamp je datetime za gRPC
                    ArriveAirportId = result.FlightDto.ArriveAirportId.ToString(),
                    ArriveDate = Timestamp.FromDateTime(result.FlightDto.ArriveDate),
                    DurationMinutes = (double)result.FlightDto.DurationMinutes,
                    FlightDate = Timestamp.FromDateTime(result.FlightDto.FlightDate),
                    Status = result.FlightDto.Status.MapFlightStatusEnumToGrpc(), // U Mongo Status je tipa string, ali u result.FlightDto je FlightStatusEnum, ali u gRCP je FlightStatusGrpc
                    Price = (double)result.FlightDto.Price,
                    FlightId = result.FlightDto.FlightId.ToString()
                }
            };

            return gRpcResponse;
        }
        public override async Task<GetAvailableSeatsResultGrpc> GetAvailableSeatsGrpc(GetAvailableSeatsRequestGrpc request, ServerCallContext context)
        {
            var availableSeats = await _sender.Send(new GetAvailableSeatsQuery(new Guid(request.FlightId)), context.CancellationToken); // = GetAvailableSeatsResult(IEnumerable<SeatDto> SeatDtos);

            var gRpcResponse = new GetAvailableSeatsResultGrpc();

            // Mapiram iz GetFlightByIdResult (C#) u GetFlightByIdResultGrpc (flight.proto) tj iz FlightDto (C#) u FlightResponse (flight.proto)
            
            foreach (var availableSeat in availableSeats.SeatDtos)
            {
                gRpcResponse.SeatDtos.Add(availableSeat.MapFromSeatDtoToGrpc());
            }

            return gRpcResponse;
        }
        public override async Task<ReserveSeatResultGrpc> ReserveSeatGrpc(ReserveSeatRequestGrpc request, ServerCallContext context)
        {
            var result = await _sender.Send(new ReserveSeatCommand(new Guid(request.FlightId), request.SeatNumber), context.CancellationToken); // = ReserveSeatResult(int SeatNumber) 

            var gRpcResponse = new ReserveSeatResultGrpc { Id = result.SeatNumber }; // Zbog gRPC, moram navesti ime polja bez obzira na tip polja

            return gRpcResponse;
        }
    }
}
