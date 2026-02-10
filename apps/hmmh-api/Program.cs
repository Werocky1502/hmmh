using Hmmh.Api.Data;
using Hmmh.Api.Extensions;
using Hmmh.Api.Middleware;
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
        // Build and run the web host.
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddApiServices(builder.Configuration, builder.Environment);

        var app = builder.Build();

        ConfigurePipeline(app);

        app.Run();
    }

    /// <summary>
    ///     Configures the HTTP request pipeline.
    /// </summary>
    /// <param name="app">Web application instance.</param>
    private static void ConfigurePipeline(WebApplication app)
    {
        // Configure middleware for the API request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors();
        app.UseMiddleware<ApiExceptionMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();

        // Apply migrations automatically so the auth tables are ready.
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<HmmhDbContext>();
            dbContext.Database.Migrate();
        }

        app.Services.SeedOpenIddictAsync().GetAwaiter().GetResult();

        app.MapControllers();
    }

}
