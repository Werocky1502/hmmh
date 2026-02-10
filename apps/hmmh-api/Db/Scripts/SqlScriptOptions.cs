namespace Hmmh.Api.Db.Scripts;

/// <summary>
///     Provides configuration for SQL script execution.
/// </summary>
public sealed class SqlScriptOptions
{
    /// <summary>
    ///     Gets or sets the relative path containing SQL scripts.
    /// </summary>
    public string ScriptsPath { get; set; } = "Db/Scripts";

    /// <summary>
    ///     Gets or sets the command timeout for script execution in seconds.
    /// </summary>
    public int CommandTimeoutSeconds { get; set; } = 60;
}
