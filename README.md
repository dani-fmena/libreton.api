# BackendAPI - .NET 8.0 C# Backend API

A multiplatform .NET 8.0 backend API with repository pattern, Entity Framework, PostgreSQL, and session-based authentication.

## Project Structure

The solution consists of three projects:

### 1. **BackendAPI.Api** - REST API
The main API project with layered architecture:
- **Controllers**: Handle HTTP requests and responses
- **Services**: Business logic layer
- **Validators**: Validation layer with database access
- **DTOs**: Data Transfer Objects
- **Middleware**: Custom authentication middleware

### 2. **BackendAPI.Console** - Database Migration Tool
Console application for database management:
- Run migrations
- Clear database
- Drop and recreate database
- Seed sample data

### 3. **BackendAPI.Shared** - Shared Library
Shared code across projects:
- Constants (error messages, validation messages, auth settings)
- Models (BaseEntity, ApiResponse)
- Utilities (PasswordHasher)

### 4. **BackendAPI.Data** - Data Access Layer
Repository pattern with Unit of Work:
- **Entities**: Database models (User, Product)
- **Repositories**: Generic repository pattern
- **UnitOfWork**: Unit of Work pattern for transaction management
- **Context**: Entity Framework DbContext

## Architecture

### Layered Architecture
1. **Controller Layer**: API endpoints (`AuthController`, `ProductsController`)
2. **Validation Layer**: Request validation with database access (`AuthValidator`, `ProductValidator`)
3. **Service Layer**: Business logic (`AuthService`, `ProductService`)
4. **Data Access Layer**: Repository pattern with UoW (`IRepository<T>`, `IUnitOfWork`)

### Key Features

#### Repository Pattern with Unit of Work
- Generic repository for CRUD operations
- Unit of Work for transaction management
- Soft delete support
- Query filtering for non-deleted entities

#### Authentication System
- Session-based authentication using memory cache
- Session expiration (30 minutes default)
- Custom authentication middleware
- Session token via `X-Session-Token` header

#### Entity Framework Configuration
- PostgreSQL database provider
- Code-first approach with migrations
- Query filters for soft deletes
- Unique indexes on username and email

## Prerequisites

- .NET 8.0 SDK
- PostgreSQL 12+
- Any IDE (Visual Studio, VS Code, Rider)

## Getting Started

### 1. Configure Database Connection

Update the connection string in:
- `src/BackendAPI.Api/appsettings.json`
- `src/BackendAPI.Console/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=backendapi;Username=postgres;Password=postgres"
  }
}
```

### 2. Run Database Migrations

Using the Console application:

```bash
cd src/BackendAPI.Console
dotnet run
```

Select option:
- **1**: Run migrations (create/update database)
- **4**: Seed sample data

Or using EF Core CLI:

```bash
cd src/BackendAPI.Api
dotnet ef migrations add InitialCreate --project ../BackendAPI.Data
dotnet ef database update
```

### 3. Run the API

```bash
cd src/BackendAPI.Api
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:7xxx`
- HTTP: `http://localhost:5xxx`
- Swagger: `https://localhost:7xxx/swagger`

## API Endpoints

### Authentication Endpoints

#### Register
```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "testuser",
  "email": "test@example.com",
  "password": "test123",
  "fullName": "Test User"
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "testuser",
  "password": "test123"
}
```

Response:
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "sessionToken": "guid-here",
    "expiresAt": "2025-10-15T12:00:00Z",
    "user": {
      "id": "guid-here",
      "username": "testuser",
      "email": "test@example.com",
      "fullName": "Test User"
    }
  }
}
```

#### Logout
```http
POST /api/auth/logout
X-Session-Token: your-session-token
```

### Product Endpoints (Require Authentication)

All product endpoints require the `X-Session-Token` header.

#### Get All Products
```http
GET /api/products
X-Session-Token: your-session-token
```

#### Get Product by ID
```http
GET /api/products/{id}
X-Session-Token: your-session-token
```

#### Create Product
```http
POST /api/products
X-Session-Token: your-session-token
Content-Type: application/json

{
  "name": "Laptop",
  "description": "High-performance laptop",
  "price": 999.99,
  "stock": 10,
  "category": "Electronics"
}
```

#### Update Product
```http
PUT /api/products/{id}
X-Session-Token: your-session-token
Content-Type: application/json

{
  "name": "Updated Laptop",
  "description": "Updated description",
  "price": 899.99,
  "stock": 15,
  "category": "Electronics"
}
```

#### Delete Product
```http
DELETE /api/products/{id}
X-Session-Token: your-session-token
```

## Database Schema

### Users Table
- `Id` (UUID, Primary Key)
- `Username` (string, unique, required)
- `Email` (string, unique, required)
- `PasswordHash` (string, required)
- `FullName` (string, optional)
- `IsActive` (boolean)
- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime?)
- `IsDeleted` (boolean)

### Products Table
- `Id` (UUID, Primary Key)
- `Name` (string, required)
- `Description` (string, optional)
- `Price` (decimal)
- `Stock` (int)
- `Category` (string, optional)
- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime?)
- `IsDeleted` (boolean)

## Sample Data

When you seed the database (Console app option 4), the following users are created:

| Username | Password | Email | Full Name |
|----------|----------|-------|-----------|
| admin | admin123 | admin@example.com | System Administrator |
| testuser | test123 | test@example.com | Test User |

## Technology Stack

- **.NET 8.0**: Latest LTS version
- **ASP.NET Core**: Web API framework
- **Entity Framework Core 9.0**: ORM
- **Npgsql**: PostgreSQL provider
- **Memory Cache**: Session management
- **Swagger/OpenAPI**: API documentation

## Configuration

### Authentication Settings
Located in `BackendAPI.Shared/Constants/AppConstants.cs`:
- Session expiration: 30 minutes
- Session key prefix: "Session_"
- Auth header name: "X-Session-Token"

### Database Settings
- Default connection string name: "DefaultConnection"
- Soft delete enabled on all entities
- UTC timestamps for all date/time fields

## Development

### Building the Solution
```bash
dotnet build
```

### Running Tests
```bash
dotnet test
```

### Creating New Migrations
```bash
cd src/BackendAPI.Api
dotnet ef migrations add MigrationName --project ../BackendAPI.Data
dotnet ef database update
```

### Adding New Entities
1. Create entity in `BackendAPI.Data/Entities/` inheriting from `BaseEntity`
2. Add DbSet to `ApplicationDbContext`
3. Configure entity in `OnModelCreating`
4. Add repository property to `IUnitOfWork` and `UnitOfWork`
5. Create migration

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
