using Hmmh.Api.Contracts;
using Hmmh.Api.Models;

namespace Hmmh.Api.Factories;

/// <summary>
///     Defines creation logic for <see cref="AccountResponse" /> payloads.
/// </summary>
public interface IAccountResponseFactory
{
    /// <summary>
    ///     Creates an account response for the supplied user.
    /// </summary>
    /// <param name="user">User entity.</param>
    /// <returns>Account response payload.</returns>
    AccountResponse Create(ApplicationUser user);
}
