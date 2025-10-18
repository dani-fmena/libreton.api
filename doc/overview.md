👓 Overview
===

## Project Structure

The solution consists of three projects:

### 1. **LibretonAPI.Api** - REST API
The main API project with layered architecture:
- **Controllers**: Handle HTTP requests and responses
- **Services**: Business logic layer
- **Validators**: Validation layer with database access
- **DTOs**: Data Transfer Objects
- **Middleware**: Custom authentication middleware

### 2. **LibretonAPI.Console** - Database Migration Tool
Console application for database management:
- Run migrations
- Clear database
- Drop and recreate database
- Seed sample data

### 3. **LibretonAPI.Shared** - Shared Library
Shared code across projects:
- Constants (error messages, validation messages, auth settings)
- Models (BaseEntity, ApiResponse)
- Utilities (PasswordHasher)

### 4. **LibretonAPI.Data** - Data Access Layer
Repository pattern with Unit of Work:
- **Entities**: Database models (User, Product)
- **Repositories**: Generic repository pattern
- **UnitOfWork**: Unit of Work pattern for transaction management
- **Context**: Entity Framework DbContext


## Technology Stack

- **.NET 8.0**: Latest LTS version
- **ASP.NET Core**: Web API framework
- **Entity Framework Core 9.0**: ORM
- **Npgsql**: PostgreSQL provider
- **Memory Cache**: Session management
- **Swagger/OpenAPI**: API documentation

## Configuration

### Authentication Settings
Located in `LibretonAPI.Shared/Constants/AppConstants.cs`:
- Session expiration: 30 minutes
- Session key prefix: "Session_"
- Auth header name: "X-Session-Token"

### Database Settings
- Default connection string name: "DefaultConnection"
- Soft delete enabled on all entities
- UTC timestamps for all date/time fields


## Security Considerations

- Passwords are hashed using SHA256
- Sessions expire after 30 minutes of inactivity
- Soft delete prevents data loss
- Input validation at controller level
- Authentication middleware protects endpoints

## License

This project is licensed under the MIT License.

## Support

For issues, questions, or contributions, please open an issue in the repository.
