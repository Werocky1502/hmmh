using Hmmh.Api.Db.Models;

namespace Hmmh.Api.Factories;

/// <summary>
///     Builds <see cref="ApplicationUser" /> entities for persistence.
/// </summary>
public sealed class ApplicationUserFactory : IApplicationUserFactory
{
    /// <inheritdoc />
    public ApplicationUser Create(string login, string passwordHash)
    {
        // Create the user with the required identifiers and credentials.
        return new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = login,
            PasswordHash = passwordHash,
        };
    }
}
