using System.Security.Cryptography;
using System.Text;
using Hmmh.Api.Db.Data;
using Hmmh.Api.Db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hmmh.Api.Db.Scripts;

/// <summary>
///     Runs ordered SQL scripts and persists execution history.
/// </summary>
public sealed class SqlScriptRunner : ISqlScriptRunner
{
    private readonly HmmhDbContext dbContext;
    private readonly SqlScriptOptions options;
    private readonly IHostEnvironment environment;
    private readonly ILogger<SqlScriptRunner> logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SqlScriptRunner" /> class.
    /// </summary>
    /// <param name="dbContext">Database context for script execution.</param>
    /// <param name="options">Script runner options.</param>
    /// <param name="environment">Hosting environment for content root.</param>
    /// <param name="logger">Logger for script execution output.</param>
    public SqlScriptRunner(
        HmmhDbContext dbContext,
        IOptions<SqlScriptOptions> options,
        IHostEnvironment environment,
        ILogger<SqlScriptRunner> logger)
    {
        this.dbContext = dbContext;
        this.options = options.Value;
        this.environment = environment;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task ApplyPendingScriptsAsync(CancellationToken cancellationToken = default)
    {
        // Ensure the schema is current before applying scripts.
        await dbContext.Database.MigrateAsync(cancellationToken);

        var scriptsDirectory = ResolveScriptsDirectory();
        if (!Directory.Exists(scriptsDirectory))
        {
            logger.LogInformation("SQL scripts directory not found at {ScriptsDirectory}.", scriptsDirectory);
            return;
        }

        var scripts = Directory.EnumerateFiles(scriptsDirectory, "*.sql", SearchOption.TopDirectoryOnly)
            .OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (scripts.Count == 0)
        {
            logger.LogInformation("No SQL scripts found in {ScriptsDirectory}.", scriptsDirectory);
            return;
        }

        dbContext.Database.SetCommandTimeout(options.CommandTimeoutSeconds);

        foreach (var scriptPath in scripts)
        {
            await ApplyScriptAsync(scriptPath, cancellationToken);
        }
    }

    private async Task ApplyScriptAsync(string scriptPath, CancellationToken cancellationToken)
    {
        // Execute a single script and record history on success.
        var scriptName = Path.GetFileName(scriptPath);
        var scriptContent = await File.ReadAllTextAsync(scriptPath, cancellationToken);

        if (string.IsNullOrWhiteSpace(scriptContent))
        {
            logger.LogInformation("Skipping empty SQL script {ScriptName}.", scriptName);
            return;
        }

        var scriptHash = ComputeHash(scriptContent);
        var existing = await dbContext.ScriptHistories
            .AsNoTracking()
            .FirstOrDefaultAsync(history => history.ScriptName == scriptName, cancellationToken);

        if (existing is not null)
        {
            if (!string.Equals(existing.ScriptHash, scriptHash, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"SQL script '{scriptName}' has changed since it was applied. Update the script name to rerun.");
            }

            logger.LogInformation("Skipping previously applied SQL script {ScriptName}.", scriptName);
            return;
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        await dbContext.Database.ExecuteSqlRawAsync(scriptContent, cancellationToken);

        dbContext.ScriptHistories.Add(new ScriptHistory
        {
            ScriptName = scriptName,
            ScriptHash = scriptHash,
            AppliedOn = DateTimeOffset.UtcNow,
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation("Applied SQL script {ScriptName}.", scriptName);
    }

    private string ResolveScriptsDirectory()
    {
        // Resolve scripts from the API content root.
        var path = options.ScriptsPath;
        if (Path.IsPathRooted(path))
        {
            return path;
        }

        return Path.GetFullPath(Path.Combine(environment.ContentRootPath, path));
    }

    private static string ComputeHash(string content)
    {
        // Compute a SHA256 hash for script tracking.
        using var sha = SHA256.Create();
        var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(content));
        var builder = new StringBuilder(hashBytes.Length * 2);

        foreach (var hashByte in hashBytes)
        {
            builder.Append(hashByte.ToString("x2"));
        }

        return builder.ToString();
    }
}
