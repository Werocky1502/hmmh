namespace Hmmh.Api.Extensions;

/// <summary>
///     Provides string normalization helpers for the API.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    ///     Normalizes a login to trimmed lowercase text.
    /// </summary>
    /// <param name="value">Login string to normalize.</param>
    /// <returns>Normalized login string.</returns>
    public static string NormalizeLogin(this string? value)
    {
        // Normalize login values to trimmed lowercase.
        return (value ?? string.Empty).Trim().ToLowerInvariant();
    }

    /// <summary>
    ///     Normalizes optional text to a trimmed value or null.
    /// </summary>
    /// <param name="value">Text value to normalize.</param>
    /// <returns>Trimmed value or null when empty.</returns>
    public static string? NormalizeOptionalText(this string? value)
    {
        // Normalize optional text input to trimmed values.
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }
}
