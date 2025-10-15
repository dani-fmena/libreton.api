namespace BackendAPI.Shared.Constants;

public static class AppConstants
{
    public static class Auth
    {
        public const string SessionKeyPrefix = "Session_";
        public const int SessionExpirationMinutes = 30;
        public const string AuthHeaderName = "X-Session-Token";
    }

    public static class Database
    {
        public const string DefaultConnectionStringName = "DefaultConnection";
    }

    public static class ErrorMessages
    {
        public const string UnauthorizedAccess = "Unauthorized access. Please login.";
        public const string InvalidCredentials = "Invalid username or password.";
        public const string SessionExpired = "Your session has expired. Please login again.";
        public const string ValidationFailed = "Validation failed.";
        public const string InternalServerError = "An internal server error occurred.";
        public const string ResourceNotFound = "The requested resource was not found.";
    }

    public static class ValidationMessages
    {
        public const string RequiredField = "{0} is required.";
        public const string InvalidEmail = "Invalid email format.";
        public const string MinLength = "{0} must be at least {1} characters.";
        public const string MaxLength = "{0} must not exceed {1} characters.";
    }
}
