using BackendAPI.Api.DTOs;
using BackendAPI.Data.Entities;
using BackendAPI.Data.UnitOfWork;
using BackendAPI.Shared.Constants;
using BackendAPI.Shared.Utilities;
using Microsoft.Extensions.Caching.Memory;

namespace BackendAPI.Api.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMemoryCache _memoryCache;

    public AuthService(IUnitOfWork unitOfWork, IMemoryCache memoryCache)
    {
        _unitOfWork = unitOfWork;
        _memoryCache = memoryCache;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => 
            u.Username == request.Username && u.IsActive);

        if (user == null || !PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
            return null;

        // Generate session token
        var sessionToken = Guid.NewGuid().ToString();
        var sessionKey = AppConstants.Auth.SessionKeyPrefix + sessionToken;
        var expiresAt = DateTime.UtcNow.AddMinutes(AppConstants.Auth.SessionExpirationMinutes);

        var userInfo = new UserInfo
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName
        };

        // Store in cache
        _memoryCache.Set(sessionKey, userInfo, new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = expiresAt
        });

        return new LoginResponse
        {
            SessionToken = sessionToken,
            ExpiresAt = expiresAt,
            User = userInfo
        };
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        var hashedPassword = PasswordHasher.HashPassword(request.Password);

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = hashedPassword,
            FullName = request.FullName
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public Task<bool> LogoutAsync(string sessionToken)
    {
        var sessionKey = AppConstants.Auth.SessionKeyPrefix + sessionToken;
        _memoryCache.Remove(sessionKey);
        return Task.FromResult(true);
    }

    public Task<UserInfo?> ValidateSessionAsync(string sessionToken)
    {
        var sessionKey = AppConstants.Auth.SessionKeyPrefix + sessionToken;
        _memoryCache.TryGetValue<UserInfo>(sessionKey, out var userInfo);
        return Task.FromResult(userInfo);
    }
}
