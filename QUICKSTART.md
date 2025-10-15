# Quick Start Guide

Get the BackendAPI up and running in minutes!

## Prerequisites

- .NET 8.0 SDK ([Download here](https://dotnet.microsoft.com/download/dotnet/8.0))
- PostgreSQL 12+ ([Download here](https://www.postgresql.org/download/))
- Your favorite IDE (Visual Studio, VS Code, or JetBrains Rider)

## Step 1: Install PostgreSQL

### Windows
1. Download PostgreSQL installer
2. Run installer and set password for `postgres` user
3. Default port: 5432

### macOS
```bash
brew install postgresql@14
brew services start postgresql@14
```

### Linux (Ubuntu/Debian)
```bash
sudo apt update
sudo apt install postgresql postgresql-contrib
sudo systemctl start postgresql
```

## Step 2: Create Database

```bash
# Connect to PostgreSQL
psql -U postgres

# Create database
CREATE DATABASE backendapi;

# Exit
\q
```

Or let the migrations create it automatically!

## Step 3: Update Connection Strings

Edit `src/BackendAPI.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=backendapi;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```

Also update `src/BackendAPI.Console/appsettings.json` with the same connection string.

## Step 4: Run Migrations

### Option A: Using Console Application (Recommended)

```bash
cd src/BackendAPI.Console
dotnet run
```

Select:
- **Option 1**: Run migrations
- **Option 4**: Seed sample data (optional)

### Option B: Using EF Core CLI

```bash
# Install EF Core CLI (if not already installed)
dotnet tool install --global dotnet-ef

# Run migrations
cd src/BackendAPI.Api
dotnet ef database update --project ../BackendAPI.Data
```

## Step 5: Run the API

```bash
cd src/BackendAPI.Api
dotnet run
```

The API will start on:
- HTTPS: `https://localhost:7xxx` (check console output for exact port)
- HTTP: `http://localhost:5xxx`

## Step 6: Test the API

### Using Swagger UI

Open your browser and navigate to:
```
https://localhost:7xxx/swagger
```

### Using the HTTP Examples File

1. Open `API_EXAMPLES.http` in VS Code (with REST Client extension)
2. Update the `@baseUrl` variable with your API URL
3. Click "Send Request" on any endpoint

### Using cURL

```bash
# Register a user
curl -X POST https://localhost:7xxx/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "email": "test@example.com",
    "password": "test123",
    "fullName": "Test User"
  }'

# Login
curl -X POST https://localhost:7xxx/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "password": "test123"
  }'

# Copy the sessionToken from the response, then:

# Get all products (replace TOKEN with your session token)
curl -X GET https://localhost:7xxx/api/products \
  -H "X-Session-Token: TOKEN"
```

## Default Users (If Seeded)

If you ran the seed data option:

| Username | Password | Role |
|----------|----------|------|
| admin | admin123 | Administrator |
| testuser | test123 | User |

## Common Issues

### Issue: "dotnet: command not found"

**Solution**: Install .NET 8.0 SDK from https://dotnet.microsoft.com/download/dotnet/8.0

### Issue: Cannot connect to PostgreSQL

**Solution**: 
1. Verify PostgreSQL is running:
   ```bash
   # Windows (in Services)
   services.msc
   
   # macOS/Linux
   sudo systemctl status postgresql
   ```
2. Check connection string credentials
3. Ensure port 5432 is not blocked

### Issue: "A connection was attempted on an unreachable network"

**Solution**: 
1. Check if PostgreSQL is listening on correct port
2. Update connection string host (try 127.0.0.1 instead of localhost)
3. Check firewall settings

### Issue: Migration fails with "permission denied"

**Solution**: 
1. Ensure the PostgreSQL user has permission to create databases
2. Try using the postgres superuser for development
3. Check PostgreSQL logs for detailed error

### Issue: "Trust anchor for the certification path was not found"

**Solution**: This is an HTTPS development certificate issue. 
```bash
# Trust the development certificate
dotnet dev-certs https --trust
```

## Next Steps

### 1. Explore the API
- Open Swagger UI
- Try all endpoints
- Test authentication flow

### 2. Add Your Own Features
- Create new entities in `BackendAPI.Data/Entities/`
- Add repositories to UnitOfWork
- Create services and controllers
- Run migrations

### 3. Deploy to Production
- Set environment variables for connection strings
- Use a strong PostgreSQL password
- Enable HTTPS only
- Configure CORS for your domain
- Set up logging and monitoring

## Project Structure Quick Reference

```
BackendAPI/
├── src/
│   ├── BackendAPI.Api/          # 🌐 REST API
│   ├── BackendAPI.Console/      # 🛠️ Migration Tool
│   ├── BackendAPI.Data/         # 💾 Data Access
│   └── BackendAPI.Shared/       # 📦 Shared Code
├── README.md                    # 📖 Main documentation
├── ARCHITECTURE.md              # 🏗️ Architecture details
├── QUICKSTART.md               # ⚡ This file
└── API_EXAMPLES.http           # 📝 Example requests
```

## Useful Commands

```bash
# Build solution
dotnet build

# Run tests (when added)
dotnet test

# Clean build artifacts
dotnet clean

# Create new migration
cd src/BackendAPI.Api
dotnet ef migrations add MigrationName --project ../BackendAPI.Data

# Update database
dotnet ef database update

# Rollback migration
dotnet ef database update PreviousMigrationName

# List migrations
dotnet ef migrations list

# Remove last migration
dotnet ef migrations remove --project ../BackendAPI.Data
```

## Development Tips

1. **Use Hot Reload**: Changes to C# files will automatically reload in .NET 8
2. **Enable Detailed Errors**: Set `ASPNETCORE_ENVIRONMENT=Development`
3. **Watch Database Changes**: Use pgAdmin or DBeaver to inspect the database
4. **Use Swagger**: Built-in API documentation and testing
5. **Check Logs**: Console output shows all HTTP requests and database queries

## Getting Help

- Check `README.md` for full documentation
- Read `ARCHITECTURE.md` for design details
- Review `API_EXAMPLES.http` for endpoint examples
- Open an issue on the repository

## Success! 🎉

You should now have:
- ✅ PostgreSQL running
- ✅ Database created and migrated
- ✅ API running on HTTPS
- ✅ Swagger UI accessible
- ✅ Test user created (if seeded)

**Try it out**: Register a new user, login, and create some products!

Happy coding! 🚀
