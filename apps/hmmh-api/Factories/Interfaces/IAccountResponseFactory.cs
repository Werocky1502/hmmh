using Hmmh.Api.Contracts.Responses;
using Hmmh.Api.Db.Models;

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
