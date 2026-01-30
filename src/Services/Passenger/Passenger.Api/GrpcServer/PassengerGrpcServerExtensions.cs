using Passenger.Api.GrpcServer.Services;

namespace Passenger.Api.GrpcServer
{
    public static class PassengerGrpcServerExtensions
    {
        public static void AddPassengerGrpcServer(this IServiceCollection services)
        {
            services.AddGrpc(); // Jer je Passenger gRPC server 
        }

        public static void MapPassengerGrpcServer(this WebApplication app)
        {
            app.MapGrpcService<PassengerGrpcEndpoints>(); // Jer je Passenger gRPC server
        }
    }
}
