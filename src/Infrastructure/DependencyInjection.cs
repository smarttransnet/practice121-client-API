using System.Text;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Infrastructure.Authentication;
using Infrastructure.Authorization;
using Infrastructure.Database;
using Infrastructure.DomainEvents;
using Infrastructure.OTP;
using Infrastructure.Time;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SharedKernel;
using HealthChecks.NpgSql;
using Npgsql;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .AddAppSettings(configuration)
            .AddServices()
            .AddDatabase(configuration)
            .AddHealthChecks(configuration)
            .AddAuthenticationInternal(configuration)
            .AddAuthorizationInternal();

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddTransient<IDomainEventsDispatcher, DomainEventsDispatcher>();

        return services;
    }

    private static IServiceCollection AddAppSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var appSettings = new Application.Abstractions.AppSettings();
        configuration.GetSection("App").Bind(appSettings);
        services.AddSingleton(appSettings);
        return services;
    }

    private static string ResolveConnectionString(string? originalConnectionString)
    {
        if (string.IsNullOrWhiteSpace(originalConnectionString))
        {
            return string.Empty;
        }

        if (!OperatingSystem.IsWindows())
        {
            var builder = new NpgsqlConnectionStringBuilder(originalConnectionString);

            if (Directory.Exists("/cloudsql"))
            {
                string[] entries = Directory.GetFileSystemEntries("/cloudsql");
                string? socketDir = entries.FirstOrDefault(e => !e.EndsWith("README", StringComparison.OrdinalIgnoreCase) && Directory.Exists(e))
                    ?? entries.FirstOrDefault(e => !e.EndsWith("README", StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrEmpty(socketDir))
                {
                    builder.Host = socketDir;
                    return builder.ConnectionString;
                }
            }

            // Fallback for Cloud Run gen2 execution environment
            builder.Host = "/cloudsql/note365:asia-southeast1:practice121fe";
            return builder.ConnectionString;
        }

        return originalConnectionString;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = ResolveConnectionString(configuration.GetConnectionString("Database"));

        services.AddDbContext<ApplicationDbContext>(
            options => options
                .UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Default);
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorCodesToAdd: null);
                })
                .UseSnakeCaseNamingConvention()
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    private static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = ResolveConnectionString(configuration.GetConnectionString("Database"));

        services
            .AddHealthChecks()
            .AddNpgSql(connectionString);

        return services;
    }

    private static IServiceCollection AddAuthenticationInternal(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!)),
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenProvider, TokenProvider>();

        // Doctor Portal services
        services.AddHttpClient();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        services.AddScoped<IOtpService, OtpService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ISmsService, SmsService>();

        return services;
    }

    private static IServiceCollection AddAuthorizationInternal(this IServiceCollection services)
    {
        services.AddAuthorization();

        services.AddScoped<PermissionProvider>();

        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        return services;
    }
}
