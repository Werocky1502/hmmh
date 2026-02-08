using Hmmh.Api.Data;
using Microsoft.EntityFrameworkCore;

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
        app.MapControllers();
    }
}
