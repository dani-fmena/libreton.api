# Architecture Documentation

## Overview

This project implements a clean, layered architecture following SOLID principles and industry best practices for .NET applications.

## Solution Structure

```
BackendAPI/
├── src/
│   ├── BackendAPI.Api/           # REST API Project
│   │   ├── Controllers/          # API endpoints
│   │   ├── Services/             # Business logic
│   │   ├── Validators/           # Validation logic with DB access
│   │   ├── DTOs/                 # Data transfer objects
│   │   ├── Middleware/           # Custom middleware
│   │   └── Program.cs            # Application entry point
│   │
│   ├── BackendAPI.Data/          # Data Access Layer
│   │   ├── Context/              # EF Core DbContext
│   │   ├── Entities/             # Database models
│   │   ├── Repositories/         # Repository pattern
│   │   └── UnitOfWork/           # Unit of Work pattern
│   │
│   ├── BackendAPI.Shared/        # Shared Library
│   │   ├── Constants/            # Application constants
│   │   ├── Models/               # Shared models
│   │   └── Utilities/            # Helper utilities
│   │
│   └── BackendAPI.Console/       # Migration Tool
│       └── Program.cs            # Database management
│
└── BackendAPI.sln                # Solution file
```

## Architectural Patterns

### 1. Layered Architecture

The application follows a strict layered architecture:

```
┌─────────────────────────────────┐
│   Controller Layer              │  ← HTTP Request/Response
├─────────────────────────────────┤
│   Validation Layer              │  ← Request Validation + DB Queries
├─────────────────────────────────┤
│   Service Layer                 │  ← Business Logic
├─────────────────────────────────┤
│   Data Access Layer (DAL)       │  ← Repository Pattern + UoW
├─────────────────────────────────┤
│   Entity Framework Core         │  ← ORM
├─────────────────────────────────┤
│   PostgreSQL Database           │  ← Data Storage
└─────────────────────────────────┘
```

**Layer Responsibilities:**

- **Controller Layer**: Receives HTTP requests, calls validators and services, returns HTTP responses
- **Validation Layer**: Validates incoming requests, can access database for validation rules
- **Service Layer**: Contains business logic, orchestrates data operations
- **DAL**: Provides data access through repositories, manages transactions via Unit of Work

### 2. Repository Pattern

The Repository Pattern abstracts data access logic and provides a collection-like interface for accessing domain objects.

**Benefits:**
- Decouples business logic from data access
- Enables unit testing with mock repositories
- Centralizes data access logic
- Provides a clean API for data operations

**Implementation:**

```csharp
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    void SoftDelete(T entity);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
}
```

**Features:**
- Generic implementation for all entities
- Soft delete support
- Expression-based queries
- Async operations

### 3. Unit of Work Pattern

The Unit of Work pattern maintains a list of objects affected by a business transaction and coordinates the writing out of changes.

**Benefits:**
- Groups multiple operations into a single transaction
- Ensures data consistency
- Reduces database round trips
- Simplifies transaction management

**Implementation:**

```csharp
public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<Product> Products { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
```

**Usage Example:**

```csharp
// Begin transaction
await _unitOfWork.BeginTransactionAsync();

try
{
    // Multiple operations
    await _unitOfWork.Users.AddAsync(user);
    await _unitOfWork.Products.AddAsync(product);
    
    // Commit all changes
    await _unitOfWork.CommitTransactionAsync();
}
catch
{
    // Rollback on error
    await _unitOfWork.RollbackTransactionAsync();
    throw;
}
```

### 4. Dependency Injection

The application uses ASP.NET Core's built-in dependency injection container.

**Service Registration (Program.cs):**

```csharp
// Data Layer
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Service Layer
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();

// Validation Layer
builder.Services.AddScoped<AuthValidator>();
builder.Services.AddScoped<ProductValidator>();

// Infrastructure
builder.Services.AddMemoryCache();
```

**Scoped Services:**
- Services are scoped to HTTP request lifetime
- DbContext is scoped to ensure proper disposal
- Validators and services share the same scope

## Authentication Flow

### Session-Based Authentication with Memory Cache

```
┌──────────┐          ┌──────────┐          ┌──────────┐
│  Client  │          │   API    │          │  Cache   │
└────┬─────┘          └────┬─────┘          └────┬─────┘
     │                     │                     │
     │  POST /auth/login   │                     │
     ├────────────────────>│                     │
     │                     │                     │
     │                     │  Validate User      │
     │                     │  (Database)         │
     │                     │                     │
     │                     │  Generate Token     │
     │                     ├────────────────────>│
     │                     │  Store Session      │
     │                     │                     │
     │  Session Token      │                     │
     │<────────────────────┤                     │
     │                     │                     │
     │  GET /api/products  │                     │
     │  X-Session-Token    │                     │
     ├────────────────────>│                     │
     │                     │  Validate Token     │
     │                     ├────────────────────>│
     │                     │  Get User Info      │
     │                     │<────────────────────┤
     │                     │                     │
     │  Response           │                     │
     │<────────────────────┤                     │
     │                     │                     │
```

**Key Components:**

1. **AuthService**: Handles login, registration, logout, session validation
2. **AuthenticationMiddleware**: Intercepts requests and validates session tokens
3. **Memory Cache**: Stores session data with automatic expiration

**Security Features:**

- Password hashing using SHA256
- Session expiration (30 minutes)
- Automatic session cleanup
- Token-based authentication via custom header

## Data Flow Example

### Creating a Product

```
HTTP POST /api/products
     ↓
[AuthenticationMiddleware]
     ↓ (validates session)
[ProductsController.Create]
     ↓ (receives request)
[ProductValidator.ValidateCreateProduct]
     ↓ (validates input)
[ProductService.CreateProductAsync]
     ↓ (business logic)
[UnitOfWork.Products.AddAsync]
     ↓ (adds to context)
[UnitOfWork.SaveChangesAsync]
     ↓ (commits to database)
[PostgreSQL Database]
     ↓
[Response] ← ← ← ← ← ← ←
```

## Entity Framework Configuration

### Database Context

The `ApplicationDbContext` configures:

- Entity mappings and relationships
- Indexes for performance
- Query filters for soft deletes
- Default values and constraints

### Soft Delete Implementation

All entities inherit from `BaseEntity` which includes:

```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
```

Global query filter ensures soft-deleted records are excluded:

```csharp
entity.HasQueryFilter(e => !e.IsDeleted);
```

### Migration Strategy

- Code-First approach
- Migrations stored in Data project
- Applied via Console tool or EF CLI
- Version controlled for team collaboration

## Error Handling

### Standardized API Responses

All endpoints return a consistent response format:

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
}
```

**Success Response:**
```json
{
  "success": true,
  "message": "Operation successful",
  "data": { ... }
}
```

**Error Response:**
```json
{
  "success": false,
  "message": "Validation failed",
  "errors": ["Error 1", "Error 2"]
}
```

## Validation Strategy

### Two-Level Validation

1. **Request-Level Validation**: Checks data format and business rules
2. **Database-Level Validation**: Checks uniqueness and referential integrity

**Example:**

```csharp
public async Task<List<string>> ValidateRegisterRequestAsync(RegisterRequest request)
{
    var errors = new List<string>();
    
    // Format validation
    if (string.IsNullOrWhiteSpace(request.Username))
        errors.Add("Username is required");
    
    // Database validation
    var existingUser = await _unitOfWork.Users
        .FirstOrDefaultAsync(u => u.Username == request.Username);
    if (existingUser != null)
        errors.Add("Username already exists");
    
    return errors;
}
```

## Scalability Considerations

### Current Implementation

- In-memory session cache (single server)
- Direct database access
- Synchronous operations where possible

### Future Enhancements

For production environments, consider:

1. **Distributed Cache**: Replace MemoryCache with Redis
2. **Message Queue**: Add RabbitMQ/Azure Service Bus for async operations
3. **API Gateway**: Add rate limiting and load balancing
4. **Logging**: Integrate Serilog or Application Insights
5. **Health Checks**: Add endpoint monitoring
6. **CQRS**: Separate read/write operations for complex domains

## Testing Strategy

### Unit Testing

Test each layer independently:

```csharp
// Repository tests with in-memory database
// Service tests with mock repositories
// Controller tests with mock services
// Validator tests with mock unit of work
```

### Integration Testing

Test the full stack:

```csharp
// Database integration tests
// API endpoint tests
// Authentication flow tests
```

## Performance Optimization

### Implemented Optimizations

1. **Async/Await**: All I/O operations are asynchronous
2. **Query Filters**: Automatic soft delete filtering
3. **Indexes**: Username and Email indexed for fast lookups
4. **Connection Pooling**: Enabled by default in Npgsql
5. **Memory Cache**: Fast session validation

### Best Practices

- Use `AsNoTracking()` for read-only queries
- Implement pagination for large datasets
- Use bulk operations for multiple inserts
- Cache frequently accessed data
- Monitor query execution plans

## Security Checklist

✅ Password hashing (SHA256)  
✅ Session expiration  
✅ SQL injection prevention (EF Core)  
✅ Input validation  
✅ Soft delete for data recovery  
✅ CORS configuration  
✅ HTTPS enforcement  

### Additional Security Recommendations

- [ ] Implement stronger password hashing (bcrypt/Argon2)
- [ ] Add rate limiting
- [ ] Implement refresh tokens
- [ ] Add request/response encryption
- [ ] Enable audit logging
- [ ] Add API versioning
- [ ] Implement JWT tokens for stateless auth

## Deployment Considerations

### Database

- Ensure PostgreSQL is installed and running
- Apply migrations before deployment
- Set strong database credentials
- Enable connection pooling
- Configure backup strategy

### Application

- Set appropriate connection strings
- Configure CORS for production domains
- Enable HTTPS only
- Set proper session timeout
- Configure logging levels
- Monitor application health

### Environment Variables

Consider externalizing:
- Connection strings
- Session timeout values
- Cache expiration settings
- API keys and secrets

## Conclusion

This architecture provides a solid foundation for building scalable, maintainable .NET applications. The separation of concerns, dependency injection, and established patterns make the codebase easy to understand, test, and extend.
