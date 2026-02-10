using Hmmh.Api.Extensions;
using Hmmh.Api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Hmmh.Api.Tests.Extensions;

/// <summary>
///     Tests for <see cref="ServiceCollectionExtensions" />.
/// </summary>
[TestClass]
public sealed class ServiceCollectionExtensionsTests
{
    /// <summary>
    ///     Ensures core services are registered by the extension.
    /// </summary>
    [TestMethod]
    public void AddApiServices_RegistersExpectedServices()
    {
        // Verify service registration of core dependencies.
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:HmmhDatabase", "Host=localhost;Database=hmmh;Username=hmmh;Password=hmmh" },
                { "AllowedOrigins:0", "http://localhost:5173" },
                { "OpenIddict:AccessTokenMinutes", "30" },
                { "OpenIddict:RefreshTokenDays", "7" },
            })
            .Build();
        var environment = new StubWebHostEnvironment
        {
            EnvironmentName = Environments.Development,
            ApplicationName = "Hmmh.Api",
            ContentRootPath = AppContext.BaseDirectory,
            ContentRootFileProvider = new NullFileProvider(),
            WebRootPath = AppContext.BaseDirectory,
            WebRootFileProvider = new NullFileProvider(),
        };

        services.AddApiServices(configuration, environment);
        var provider = services.BuildServiceProvider();

        Assert.IsNotNull(provider.GetService<IAuthService>());
        Assert.IsNotNull(provider.GetService<ICalorieService>());
        Assert.IsNotNull(provider.GetService<IWeightService>());
        Assert.IsNotNull(provider.GetService<IPasswordHasherService>());
    }

    private sealed class StubWebHostEnvironment : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = string.Empty;

        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();

        public string WebRootPath { get; set; } = string.Empty;

        public string EnvironmentName { get; set; } = string.Empty;

        public string ContentRootPath { get; set; } = string.Empty;

        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
