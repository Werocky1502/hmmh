using Hmmh.Api.Data;
using Hmmh.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Hmmh.Api;

public static class Program
{
    /// <summary>
    ///     Application entry point for the HMMH API.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();

        ConfigurePipeline(app);

        app.Run();
    }

    /// <summary>
    ///     Registers framework and application services.
    /// </summary>
    /// <param name="services">Service collection for dependency injection.</param>
    /// <param name="configuration">Application configuration.</param>
    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Register MVC controllers to follow REST API conventions.
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // Configure EF Core with PostgreSQL for the main application database.
        services.AddDbContext<HmmhDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("HmmhDatabase")));

        // Configure Identity for login/password authentication only.
        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = false;
            })
            .AddSignInManager()
            .AddEntityFrameworkStores<HmmhDbContext>();

        // Bind JWT settings from configuration.
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>() ?? new JwtSettings();
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = signingKey,
                    ClockSkew = TimeSpan.FromMinutes(2),
                };
            });

        services.AddAuthorization();

        // Allow the UI app to call the API from configured origins.
        var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
                policy.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod());
        });
    }

    /// <summary>
    ///     Configures the HTTP request pipeline.
    /// </summary>
    /// <param name="app">Web application instance.</param>
    private static void ConfigurePipeline(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();

        // Apply migrations automatically so the auth tables are ready.
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<HmmhDbContext>();
            dbContext.Database.Migrate();
        }

        app.MapControllers();
    }
}
