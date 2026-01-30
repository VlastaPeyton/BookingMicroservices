
using Flight.Api.GrpcServer.Services;

namespace Flight.Api.GrpcServer
{
    public static class FlightGrcpServerExtensions
    {
        public static void AddFlightGrpcServer(this IServiceCollection services)
        {
            services.AddGrpc(); // Jer je Flight gRPC server 
        }

        public static void MapFlightGrpcServer(this WebApplication app)
        {
            app.MapGrpcService<FlightGrpcEndpoints>(); // Jer je Flight gRPC server
        }
    }
}
