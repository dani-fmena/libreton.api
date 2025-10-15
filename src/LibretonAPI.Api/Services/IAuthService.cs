using LibretonAPI.Api.DTOs;

namespace LibretonAPI.Api.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<bool> RegisterAsync(RegisterRequest request);
    Task<bool> LogoutAsync(string sessionToken);
    Task<UserInfo?> ValidateSessionAsync(string sessionToken);
}
