using Hmmh.Api.Models;

namespace Hmmh.Api.Factories;

/// <summary>
///     Defines creation logic for <see cref="ApplicationUser" /> entities.
/// </summary>
public interface IApplicationUserFactory
{
    /// <summary>
    ///     Creates a new application user entity.
    /// </summary>
    /// <param name="login">Normalized login name.</param>
    /// <param name="passwordHash">Password hash payload.</param>
    /// <returns>New application user entity.</returns>
    ApplicationUser Create(string login, string passwordHash);
}
