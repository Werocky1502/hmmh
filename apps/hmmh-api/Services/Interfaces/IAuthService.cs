using Hmmh.Api.Contracts;
using Hmmh.Api.Models;

namespace Hmmh.Api.Services;

/// <summary>
///     Defines authentication-related workflows for the API.
/// </summary>
public interface IAuthService
{
    /// <summary>
    ///     Registers a new user account.
    /// </summary>
    /// <param name="request">Sign-up request payload.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>Account response for the new user.</returns>
    Task<AccountResponse> SignUpAsync(AuthRequest request, CancellationToken cancellationToken);

    /// <summary>
    ///     Deletes an existing user account.
    /// </summary>
    /// <param name="userId">Identifier of the account to remove.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>Task representing the operation.</returns>
    Task DeleteAccountAsync(Guid userId, CancellationToken cancellationToken);
}
