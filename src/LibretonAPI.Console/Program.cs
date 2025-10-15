using LibretonAPI.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

Console.WriteLine("=== LibretonAPI Database Migration Tool ===");
Console.WriteLine();

// Build configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var connectionString = configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Error: Connection string not found in appsettings.json");
    Console.ResetColor();
    return;
}

// Create DbContext options
var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
optionsBuilder.UseNpgsql(connectionString);

using var context = new ApplicationDbContext(optionsBuilder.Options);

Console.WriteLine("Available options:");
Console.WriteLine("1. Run migrations (create/update database)");
Console.WriteLine("2. Clear database (delete all data)");
Console.WriteLine("3. Drop and recreate database");
Console.WriteLine("4. Seed sample data");
Console.WriteLine("5. Exit");
Console.WriteLine();
Console.Write("Select an option (1-5): ");

var option = Console.ReadLine();

try
{
    switch (option)
    {
        case "1":
            Console.WriteLine();
            Console.WriteLine("Running migrations...");
            await context.Database.MigrateAsync();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Migrations completed successfully!");
            Console.ResetColor();
            break;

        case "2":
            Console.WriteLine();
            Console.Write("Are you sure you want to clear all data? (yes/no): ");
            var clearConfirm = Console.ReadLine()?.ToLower();
            if (clearConfirm == "yes")
            {
                Console.WriteLine("Clearing database...");
                await context.Users.ExecuteDeleteAsync();
                await context.Products.ExecuteDeleteAsync();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Database cleared successfully!");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("Operation cancelled.");
            }
            break;

        case "3":
            Console.WriteLine();
            Console.Write("Are you sure you want to drop and recreate the database? (yes/no): ");
            var dropConfirm = Console.ReadLine()?.ToLower();
            if (dropConfirm == "yes")
            {
                Console.WriteLine("Dropping database...");
                await context.Database.EnsureDeletedAsync();
                Console.WriteLine("Creating database...");
                await context.Database.MigrateAsync();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Database recreated successfully!");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("Operation cancelled.");
            }
            break;

        case "4":
            Console.WriteLine();
            Console.WriteLine("Seeding sample data...");
            await SeedDataAsync(context);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Sample data seeded successfully!");
            Console.ResetColor();
            break;

        case "5":
            Console.WriteLine("Exiting...");
            break;

        default:
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid option selected.");
            Console.ResetColor();
            break;
    }
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Details: {ex.InnerException?.Message}");
    Console.ResetColor();
}

static async Task SeedDataAsync(ApplicationDbContext context)
{
    // Check if data already exists
    if (await context.Users.AnyAsync() || await context.Products.AnyAsync())
    {
        Console.WriteLine("Data already exists. Skipping seed.");
        return;
    }

    // Seed users
    var users = new[]
    {
        new LibretonAPI.Data.Entities.User
        {
            Username = "admin",
            Email = "admin@example.com",
            PasswordHash = LibretonAPI.Shared.Utilities.PasswordHasher.HashPassword("admin123"),
            FullName = "System Administrator",
            IsActive = true
        },
        new LibretonAPI.Data.Entities.User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = LibretonAPI.Shared.Utilities.PasswordHasher.HashPassword("test123"),
            FullName = "Test User",
            IsActive = true
        }
    };

    await context.Users.AddRangeAsync(users);

    // Seed products
    var products = new[]
    {
        new LibretonAPI.Data.Entities.Product
        {
            Name = "Laptop",
            Description = "High-performance laptop",
            Price = 999.99m,
            Stock = 10,
            Category = "Electronics"
        },
        new LibretonAPI.Data.Entities.Product
        {
            Name = "Mouse",
            Description = "Wireless mouse",
            Price = 29.99m,
            Stock = 50,
            Category = "Electronics"
        },
        new LibretonAPI.Data.Entities.Product
        {
            Name = "Keyboard",
            Description = "Mechanical keyboard",
            Price = 79.99m,
            Stock = 30,
            Category = "Electronics"
        }
    };

    await context.Products.AddRangeAsync(products);
    await context.SaveChangesAsync();
}
