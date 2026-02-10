namespace Hmmh.Api.Db.Scripts;

/// <summary>
///     Executes SQL scripts and tracks which ones were applied.
/// </summary>
public interface ISqlScriptRunner
{
    /// <summary>
    ///     Applies pending SQL scripts from the configured scripts folder.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>Task representing the async operation.</returns>
    Task ApplyPendingScriptsAsync(CancellationToken cancellationToken = default);
}
