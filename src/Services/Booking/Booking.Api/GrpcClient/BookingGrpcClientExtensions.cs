using Flight;

namespace Booking.Api.GrpcClient
{
    public static class BookingGrpcClientExtensions
    {
        public static void AddBookingGrpcClient(this IServiceCollection services)
        {
            services.AddGrpcClient<FlightGrpcService.FlightGrpcServiceClient>(options =>
            {
                options.Address = new Uri("https://localhost:7034"); // Booking microservice gRCP Client poziva Flight microservice gRCP Server koji je na ovoj adresi
            });
        }
    }
}
