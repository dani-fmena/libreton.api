using LibretonAPI.Api.Middleware;
using LibretonAPI.Api.Services;
using LibretonAPI.Api.Validators;
using LibretonAPI.Data.Context;
using LibretonAPI.Data.UnitOfWork;
using LibretonAPI.Shared.Constants;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Memory Cache for session management
builder.Services.AddMemoryCache();

// Configure PostgreSQL Database
var connectionString = builder.Configuration.GetConnectionString(AppConstants.Database.DefaultConnectionStringName);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();

// Register Validators
builder.Services.AddScoped<AuthValidator>();
builder.Services.AddScoped<ProductValidator>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

// Use custom authentication middleware
app.UseMiddleware<AuthenticationMiddleware>();

app.MapControllers();

app.Run();
