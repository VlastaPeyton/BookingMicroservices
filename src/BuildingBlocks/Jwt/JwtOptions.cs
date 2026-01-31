namespace BuildingBlocks.Jwt
{   
    // Options pattern 
    public class JwtOptions
    {
        public string Authority { get; set; } = null!;  // URL of Identity Server microservisa
        public string Audience { get; set; } = null!;   // Microservis ciji endpoont pozivam (non-IdentityServer)
    }
}
