namespace BuildingBlocks.Jwt
{
    public class JwtOptions
    {
        public string Authority { get; set; } = null!;  // URL Identity Server microservisa
        public string Audience { get; set; } = null!;   // API scope/resource
    }
}
