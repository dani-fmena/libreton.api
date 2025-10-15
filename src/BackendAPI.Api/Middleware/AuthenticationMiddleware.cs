using BackendAPI.Api.Services;
using BackendAPI.Shared.Constants;

namespace BackendAPI.Api.Middleware;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAuthService authService)
    {
        // Skip authentication for auth endpoints
        var path = context.Request.Path.Value?.ToLower() ?? "";
        if (path.Contains("/auth/login") || path.Contains("/auth/register") || path.Contains("/swagger"))
        {
            await _next(context);
            return;
        }

        // Get session token from header
        if (!context.Request.Headers.TryGetValue(AppConstants.Auth.AuthHeaderName, out var sessionToken))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { message = AppConstants.ErrorMessages.UnauthorizedAccess });
            return;
        }

        // Validate session
        var userInfo = await authService.ValidateSessionAsync(sessionToken!);
        if (userInfo == null)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { message = AppConstants.ErrorMessages.SessionExpired });
            return;
        }

        // Store user info in HttpContext
        context.Items["User"] = userInfo;

        await _next(context);
    }
}
