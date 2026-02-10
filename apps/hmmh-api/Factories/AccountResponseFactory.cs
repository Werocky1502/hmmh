using Hmmh.Api.Contracts.Responses;
using Hmmh.Api.Db.Models;

namespace Hmmh.Api.Factories;

/// <summary>
///     Builds <see cref="AccountResponse" /> payloads for the API.
/// </summary>
public sealed class AccountResponseFactory : IAccountResponseFactory
{
    /// <inheritdoc />
    public AccountResponse Create(ApplicationUser user)
    {
        // Map the user entity to the account response payload.
        return new AccountResponse
        {
            UserName = user.UserName,
        };
    }
}
