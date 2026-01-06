namespace EHS.Domain.Settings
{
    public class JWTOptions
    {
        public string Key { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public double ExpirationInMin { get; set; }

        public double RefreshTokenExpirationInMin { get; set; }
    }
}