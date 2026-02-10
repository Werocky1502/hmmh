using OpenIddict.Abstractions;

namespace Hmmh.Api.Extensions;

/// <summary>
///     Provides service provider extension methods for startup tasks.
/// </summary>
public static class ServiceProviderExtensions
{
    /// <summary>
    ///     Ensures OpenIddict clients are registered for local development.
    /// </summary>
    /// <param name="services">Root service provider.</param>
    /// <returns>Task representing the async operation.</returns>
    public static async Task SeedOpenIddictAsync(this IServiceProvider services)
    {
        // Ensure the OpenIddict client registration exists.
        using var scope = services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        var existing = await manager.FindByClientIdAsync("hmmh-ui");
        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = "hmmh-ui",
            DisplayName = "HMMH UI",
            ClientType = OpenIddictConstants.ClientTypes.Public,
            ConsentType = OpenIddictConstants.ConsentTypes.Implicit,
        };

        descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);
        descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.Password);
        descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.RefreshToken);
        descriptor.Permissions.Add(OpenIddictConstants.Permissions.Prefixes.Scope + OpenIddictConstants.Scopes.OpenId);
        descriptor.Permissions.Add(OpenIddictConstants.Permissions.Prefixes.Scope + OpenIddictConstants.Scopes.OfflineAccess);
        descriptor.Permissions.Add(OpenIddictConstants.Permissions.Prefixes.Scope + "api");

        if (existing is null)
        {
            await manager.CreateAsync(descriptor);
            return;
        }

        await manager.UpdateAsync(existing, descriptor);
    }
}
