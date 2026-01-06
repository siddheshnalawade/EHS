namespace EHS.Application.DTOs
{
    public class AuthResponse
    {
        public string Message { get; set; }

        public bool IsAuthenticated { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public List<string> Roles { get; set; }

        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public DateTime RefreshTokenExpiration { get; set; }
    }
}