using Hmmh.Api.Db.Data;
using Hmmh.Api.Db.Repositories;
using Hmmh.Api.Db.Scripts;
using Hmmh.Api.Factories;
using Hmmh.Api.OpenApi;
using Hmmh.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;

namespace Hmmh.Api.Extensions;

/// <summary>
///     Provides dependency injection registration helpers for the API.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Registers framework and application services for the API.
    /// </summary>
    /// <param name="services">Service collection for dependency injection.</param>
    /// <param name="configuration">Application configuration.</param>
    /// <param name="environment">Hosting environment for conditional setup.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        // Register MVC controllers and API tooling.
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "HMMH API",
                Version = "v1",
                Description = "HelpMeManageHealth REST API",
            });

            var bearerScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Use a bearer token from /connect/token.",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                },
            };

            options.AddSecurityDefinition("Bearer", bearerScheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                [bearerScheme] = Array.Empty<string>(),
            });

            options.OperationFilter<TokenEndpointOperationFilter>();
        });
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();

        // Configure EF Core with PostgreSQL for the main application database.
        services.AddDbContext<HmmhDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("HmmhDatabase")));

        // Register SQL script runner configuration.
        services.Configure<SqlScriptOptions>(configuration.GetSection("SqlScripts"));
        services.AddScoped<ISqlScriptRunner, SqlScriptRunner>();

        // Register generic repository for data access.
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

        // Register entity factories for consistent creation logic.
        services.AddSingleton<IApplicationUserFactory, ApplicationUserFactory>();
        services.AddSingleton<IWeightEntryFactory, WeightEntryFactory>();
        services.AddSingleton<ICalorieEntryFactory, CalorieEntryFactory>();
        services.AddSingleton<IAccountResponseFactory, AccountResponseFactory>();
        services.AddSingleton<IWeightEntryResponseFactory, WeightEntryResponseFactory>();
        services.AddSingleton<ICalorieEntryResponseFactory, CalorieEntryResponseFactory>();
        services.AddSingleton<IStatusResponseFactory, StatusResponseFactory>();

        // Register application services for business workflows.
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IWeightService, WeightService>();
        services.AddScoped<ICalorieService, CalorieService>();

        // Provide a local password hashing service for user credentials.
        services.AddSingleton<IPasswordHasherService, PasswordHasherService>();

        var accessTokenMinutes = configuration.GetValue("OpenIddict:AccessTokenMinutes", 30);
        var refreshTokenDays = configuration.GetValue("OpenIddict:RefreshTokenDays", 30);

        services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                    .UseDbContext<HmmhDbContext>();
            })
            .AddServer(options =>
            {
                options.SetTokenEndpointUris("/connect/token");
                options.AllowPasswordFlow();
                options.AllowRefreshTokenFlow();
                options.AcceptAnonymousClients();
                options.RegisterScopes(OpenIddictConstants.Scopes.OpenId, "api", OpenIddictConstants.Scopes.OfflineAccess);
                options.SetAccessTokenLifetime(TimeSpan.FromMinutes(accessTokenMinutes));
                options.SetRefreshTokenLifetime(TimeSpan.FromDays(refreshTokenDays));

                options.AddDevelopmentEncryptionCertificate();
                options.AddDevelopmentSigningCertificate();

                var aspNetCore = options.UseAspNetCore()
                    .EnableTokenEndpointPassthrough();

                if (environment.IsDevelopment())
                {
                    aspNetCore.DisableTransportSecurityRequirement();
                }
            })
            .AddValidation(options =>
            {
                options.UseLocalServer();
                options.UseAspNetCore();
            });

        services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim(OpenIddictConstants.Claims.Subject)
                .Build();
        });

        // Allow the UI app to call the API from configured origins.
        var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
                policy.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod());
        });

        return services;
    }
}
