namespace Hmmh.Api.Models;

/// <summary>
///     Represents a local HMMH user stored in the database.
/// </summary>
public sealed class ApplicationUser
{
	/// <summary>
	///     Unique identifier for the user.
	/// </summary>
	public Guid Id { get; set; } = Guid.NewGuid();

	/// <summary>
	///     Login identifier for the user.
	/// </summary>
	public string UserName { get; set; } = string.Empty;

	/// <summary>
	///     PBKDF2 password hash stored as metadata + salt + hash.
	/// </summary>
	public string PasswordHash { get; set; } = string.Empty;
}
