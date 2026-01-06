using EHS.Application.DTOs;

namespace EHS.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest registerRequest);

        Task<AuthResponse> LoginAsync(LoginRequest loginRequest);

        Task<AuthResponse> RefreshTokenAsync(string token);

        Task<bool> RevokeTokenAsync(string token);
    }
}