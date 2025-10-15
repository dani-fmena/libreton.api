using BackendAPI.Api.DTOs;
using BackendAPI.Data.UnitOfWork;
using BackendAPI.Shared.Constants;

namespace BackendAPI.Api.Validators;

public class AuthValidator
{
    private readonly IUnitOfWork _unitOfWork;

    public AuthValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<List<string>> ValidateLoginRequestAsync(LoginRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Username))
            errors.Add(string.Format(AppConstants.ValidationMessages.RequiredField, "Username"));

        if (string.IsNullOrWhiteSpace(request.Password))
            errors.Add(string.Format(AppConstants.ValidationMessages.RequiredField, "Password"));

        return Task.FromResult(errors);
    }

    public async Task<List<string>> ValidateRegisterRequestAsync(RegisterRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Username))
        {
            errors.Add(string.Format(AppConstants.ValidationMessages.RequiredField, "Username"));
        }
        else if (request.Username.Length < 3)
        {
            errors.Add(string.Format(AppConstants.ValidationMessages.MinLength, "Username", 3));
        }
        else if (request.Username.Length > 50)
        {
            errors.Add(string.Format(AppConstants.ValidationMessages.MaxLength, "Username", 50));
        }
        else
        {
            // Check if username already exists
            var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (existingUser != null)
                errors.Add("Username is already taken.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            errors.Add(string.Format(AppConstants.ValidationMessages.RequiredField, "Email"));
        }
        else if (!IsValidEmail(request.Email))
        {
            errors.Add(AppConstants.ValidationMessages.InvalidEmail);
        }
        else
        {
            // Check if email already exists
            var existingEmail = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingEmail != null)
                errors.Add("Email is already registered.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            errors.Add(string.Format(AppConstants.ValidationMessages.RequiredField, "Password"));
        }
        else if (request.Password.Length < 6)
        {
            errors.Add(string.Format(AppConstants.ValidationMessages.MinLength, "Password", 6));
        }

        return errors;
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
