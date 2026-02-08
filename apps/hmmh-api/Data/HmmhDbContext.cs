using Hmmh.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hmmh.Api.Data;

/// <summary>
///     EF Core database context for HMMH data.
/// </summary>
public sealed class HmmhDbContext : IdentityDbContext<ApplicationUser>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="HmmhDbContext" /> class.
    /// </summary>
    /// <param name="options">Configured DbContext options.</param>
    public HmmhDbContext(DbContextOptions<HmmhDbContext> options)
        : base(options)
    {
        // Forward configuration to the base IdentityDbContext.
    }

    /// <summary>
    ///     Configures EF Core model mappings.
    /// </summary>
    /// <param name="modelBuilder">Model builder for entity configuration.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Keep Identity defaults and extend with application-specific mappings later.
        base.OnModelCreating(modelBuilder);
    }
}
