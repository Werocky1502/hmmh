using Hmmh.Api.Contracts;
using Hmmh.Api.Exceptions;
using Hmmh.Api.Factories;
using Hmmh.Api.Models;
using Hmmh.Api.Repositories;
using Hmmh.Api.Extensions;

namespace Hmmh.Api.Services;

/// <summary>
///     Handles authentication workflows for the API.
/// </summary>
public sealed class AuthService : IAuthService
{
    private readonly IRepository<ApplicationUser> userRepository;
    private readonly IApplicationUserFactory userFactory;
    private readonly IAccountResponseFactory responseFactory;
    private readonly IPasswordHasherService passwordHasher;
    private readonly ILogger<AuthService> logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AuthService" /> class.
    /// </summary>
    /// <param name="userRepository">Repository for user data.</param>
    /// <param name="userFactory">Factory for user entities.</param>
    /// <param name="passwordHasher">Password hashing service.</param>
    /// <param name="logger">Logger for auth operations.</param>
    public AuthService(
        IRepository<ApplicationUser> userRepository,
        IApplicationUserFactory userFactory,
        IAccountResponseFactory responseFactory,
        IPasswordHasherService passwordHasher,
        ILogger<AuthService> logger)
    {
        this.userRepository = userRepository;
        this.userFactory = userFactory;
        this.responseFactory = responseFactory;
        this.passwordHasher = passwordHasher;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task<AccountResponse> SignUpAsync(AuthRequest request, CancellationToken cancellationToken)
    {
        // Validate and normalize the requested login.
        var login = request.Login.NormalizeLogin();
        if (string.IsNullOrWhiteSpace(login))
        {
            throw new ValidationApiException("Login is required.");
        }

        var exists = await userRepository.ExistsAsync(user => user.UserName == login, cancellationToken);
        if (exists)
        {
            throw new ConflictApiException("Login already exists.");
        }

        var passwordHash = passwordHasher.HashPassword(request.Password);
        var user = userFactory.Create(login, passwordHash);
        await userRepository.AddAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Created new user {Login}.", login);

        return responseFactory.Create(user);
    }

    /// <inheritdoc />
    public async Task DeleteAccountAsync(Guid userId, CancellationToken cancellationToken)
    {
        // Delete the account if it exists.
        var user = await userRepository.FindAsync(userEntry => userEntry.Id == userId, true, cancellationToken);
        if (user is null)
        {
            throw new NotFoundApiException("Account does not exists.");
        }

        userRepository.Remove(user);
        await userRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Deleted account {Login}.", user.UserName);
    }

}
