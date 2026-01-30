namespace BuildingBlocks.MassTransit
{
    // Konfiguracija za RabbitMQ - cita se iz appsettings.json sekcije "RabbitMqOptions"
    public class RabbitMqOptions
    {
        public string HostName { get; set; } = "localhost";
        public ushort Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string? ExchangeName { get; set; }
    }
}
