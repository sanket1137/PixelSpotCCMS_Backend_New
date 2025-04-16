using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PixelSpot.Application.Interfaces;
using PixelSpot.Application.Services;
using PixelSpot.Domain.Interfaces;
using PixelSpot.Infrastructure.Data.Contexts;
using PixelSpot.Infrastructure.Data.Repositories;
using PixelSpot.Infrastructure.FileStorage;
using PixelSpot.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace PixelSpot.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure DbContext with PostgreSQL
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
            );

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IScreenRepository, ScreenRepository>();
        services.AddScoped<ICampaignRepository, CampaignRepository>();

        // Register JWT services
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.AddScoped<PixelSpot.Application.Interfaces.IJwtTokenService, JwtTokenService>();

        // Register file storage service
        services.Configure<FileStorageSettings>(configuration.GetSection("FileStorage"));
        services.AddScoped<IFileStorageService, FileStorageService>();
        
        // Configure JWT Authentication
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];
        
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey ?? "PixelSpot_Secure_JWT_Key_For_Authentication_And_Authorization_2023"))
            };
        });

        return services;
    }
}
