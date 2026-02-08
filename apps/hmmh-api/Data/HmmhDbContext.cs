using Microsoft.EntityFrameworkCore;

namespace Hmmh.Api.Data;

/// <summary>
///     EF Core database context for HMMH data.
/// </summary>
public sealed class HmmhDbContext : DbContext
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="HmmhDbContext" /> class.
    /// </summary>
    /// <param name="options">Configured DbContext options.</param>
    public HmmhDbContext(DbContextOptions<HmmhDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    ///     Configures EF Core model mappings.
    /// </summary>
    /// <param name="modelBuilder">Model builder for entity configuration.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
