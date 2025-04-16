using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PixelSpot.API.Filters;
using PixelSpot.API.Middleware;
using PixelSpot.Application.DTOs;
using PixelSpot.Application.Interfaces;
using PixelSpot.Application.Mappings;
using PixelSpot.Application.Services;
using PixelSpot.Application.Validation;
using PixelSpot.Infrastructure;
using Serilog;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

// Configure database connection from environment variables
// Set up PostgreSQL connection string
try
{
    // Check if there's a connection string in appsettings.json
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
    // If not found or running in production/deployment environment, try to use environment variables
    if (string.IsNullOrEmpty(connectionString) || !builder.Environment.IsDevelopment())
    {
        // Parse Replit's DATABASE_URL if available
        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        
        if (!string.IsNullOrEmpty(databaseUrl))
        {
            // Convert the URL format to a standard Npgsql connection string
            // DATABASE_URL format: postgresql://username:password@host:port/database?sslmode=require
            try 
            {
                // Parse the URL using Uri class
                var uri = new Uri(databaseUrl);
                var userInfo = uri.UserInfo.Split(':');
                var host = uri.Host;
                var dbPort = uri.Port > 0 ? uri.Port : 5432;
                var database = uri.AbsolutePath.TrimStart('/');
                var sslMode = uri.Query.Contains("sslmode=require") ? "Require" : "Prefer";
                
                var username = userInfo.Length > 0 ? Uri.UnescapeDataString(userInfo[0]) : "";
                var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
                
                // Create proper Npgsql connection string
                connectionString = $"Host={host};Port={dbPort};Database={database};Username={username};Password={password};SslMode={sslMode};";
                Console.WriteLine($"Successfully parsed DATABASE_URL into Npgsql connection string");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing DATABASE_URL: {ex.Message}");
                // Fall back to individual environment variables
            }
        }
        
        // If connectionString is still null or empty, try individual environment variables
        if (string.IsNullOrEmpty(connectionString))
        {
            var pgHost = Environment.GetEnvironmentVariable("PGHOST") ?? "localhost";
            var pgPort = Environment.GetEnvironmentVariable("PGPORT") ?? "5432";
            var pgDatabase = Environment.GetEnvironmentVariable("PGDATABASE") ?? "postgres";
            var pgUser = Environment.GetEnvironmentVariable("PGUSER") ?? "postgres";
            var pgPassword = Environment.GetEnvironmentVariable("PGPASSWORD") ?? "postgres";
            
            // Create standard PostgreSQL connection string
            connectionString = $"Host={pgHost};Port={pgPort};Database={pgDatabase};Username={pgUser};Password={pgPassword};";
        }
        
        builder.Configuration.GetSection("ConnectionStrings")["DefaultConnection"] = connectionString;
        
        // Log connection info without sensitive data
        Console.WriteLine($"Using PostgreSQL connection to host: {connectionString.Split(';').FirstOrDefault(s => s.StartsWith("Host="))?.Substring(5) ?? "unknown"}");
    }
    else
    {
        Console.WriteLine("Using PostgreSQL connection string from configuration.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error setting up database connection: {ex.Message}");
}

// Add Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Configure services
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilterAttribute>();
})
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "PixelSpot API",
        Version = "v1",
        Description = "Digital Advertising Platform for Screen Management",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "PixelSpot Support",
            Email = "support@pixelspot.com"
        }
    });
    
    // Configure JWT authorization in Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add application services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IScreenService, ScreenService>();
builder.Services.AddScoped<ICampaignService, CampaignService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Add infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Configure FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserDtoValidator>();

// Configure API behavior options
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Configure CORS
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

// Configure middleware
app.UseCustomExceptionHandler();

// Configure the HTTP request pipeline.
// Use CORS first
app.UseCors("AllowAll");

// Enable Swagger in all environments for API documentation
app.UseSwagger();

// Configure Swagger UI
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "PixelSpot API v1");
    options.RoutePrefix = "swagger"; // Keep Swagger UI at /swagger path
    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
    options.EnableFilter();
    options.DisplayRequestDuration();
});

// Configure static files with CORS headers
var staticFileOptions = new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // Add proper CORS headers
        ctx.Context.Response.Headers["Access-Control-Allow-Origin"] = "*";
        ctx.Context.Response.Headers["Access-Control-Allow-Methods"] = "GET, HEAD, OPTIONS";
        ctx.Context.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type, Authorization";
    }
};

// Make sure static files are served before routing
app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = new List<string> { "index.html" }
});
app.UseStaticFiles(staticFileOptions);

// Disable HTTPS redirection for Replit environment
// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Ensure the database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<PixelSpot.Infrastructure.Data.Contexts.ApplicationDbContext>();
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating the database.");
    }
}

// Allow using the PORT environment variable (Replit's preferred way)
var serverPort = Environment.GetEnvironmentVariable("PORT") ?? "5000";
Console.WriteLine($"Starting server on port {serverPort}");
app.Urls.Clear();
app.Urls.Add($"http://0.0.0.0:{serverPort}");

app.Run();
