namespace ClientsWebApi.Settings
{
    public class JwtSettings
    {
        public string? SigningKey { get; set; }

        public string? Issuer { get; set; }

        public int? ExpiryInMinutes { get; set; }
    }
}
