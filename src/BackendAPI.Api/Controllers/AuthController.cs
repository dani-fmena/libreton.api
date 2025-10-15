using BackendAPI.Api.DTOs;
using BackendAPI.Api.Services;
using BackendAPI.Api.Validators;
using BackendAPI.Shared.Constants;
using BackendAPI.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly AuthValidator _authValidator;

    public AuthController(IAuthService authService, AuthValidator authValidator)
    {
        _authService = authService;
        _authValidator = authValidator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<bool>>> Register([FromBody] RegisterRequest request)
    {
        var validationErrors = await _authValidator.ValidateRegisterRequestAsync(request);
        if (validationErrors.Any())
        {
            return BadRequest(ApiResponse<bool>.ErrorResponse(
                AppConstants.ErrorMessages.ValidationFailed, 
                validationErrors));
        }

        var result = await _authService.RegisterAsync(request);
        if (!result)
        {
            return BadRequest(ApiResponse<bool>.ErrorResponse("Registration failed"));
        }

        return Ok(ApiResponse<bool>.SuccessResponse(true, "User registered successfully"));
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        var validationErrors = await _authValidator.ValidateLoginRequestAsync(request);
        if (validationErrors.Any())
        {
            return BadRequest(ApiResponse<LoginResponse>.ErrorResponse(
                AppConstants.ErrorMessages.ValidationFailed, 
                validationErrors));
        }

        var result = await _authService.LoginAsync(request);
        if (result == null)
        {
            return Unauthorized(ApiResponse<LoginResponse>.ErrorResponse(
                AppConstants.ErrorMessages.InvalidCredentials));
        }

        return Ok(ApiResponse<LoginResponse>.SuccessResponse(result, "Login successful"));
    }

    [HttpPost("logout")]
    public async Task<ActionResult<ApiResponse<bool>>> Logout()
    {
        if (!Request.Headers.TryGetValue(AppConstants.Auth.AuthHeaderName, out var sessionToken))
        {
            return BadRequest(ApiResponse<bool>.ErrorResponse("Session token not provided"));
        }

        await _authService.LogoutAsync(sessionToken!);
        return Ok(ApiResponse<bool>.SuccessResponse(true, "Logout successful"));
    }
}
