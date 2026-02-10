namespace Hmmh.Api.Db.Models;

/// <summary>
///     Tracks applied SQL scripts to prevent re-execution.
/// </summary>
public sealed class ScriptHistory
{
    /// <summary>
    ///     Gets or sets the unique script file name.
    /// </summary>
    public string ScriptName { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the hash of the applied script contents.
    /// </summary>
    public string ScriptHash { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the UTC timestamp when the script was applied.
    /// </summary>
    public DateTimeOffset AppliedOn { get; set; }
}
