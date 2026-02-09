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
    ///     Weight entries recorded by users.
    /// </summary>
    public DbSet<WeightEntry> WeightEntries => Set<WeightEntry>();

    /// <summary>
    ///     Configures EF Core model mappings.
    /// </summary>
    /// <param name="modelBuilder">Model builder for entity configuration.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Keep Identity defaults and extend with application-specific mappings later.
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WeightEntry>(entity =>
        {
            // Enforce one weight record per user per date with consistent precision.
            entity.HasKey(entry => entry.Id);
            entity.Property(entry => entry.EntryDate).HasColumnType("date");
            entity.Property(entry => entry.WeightKg).HasPrecision(5, 2);
            entity.HasIndex(entry => new { entry.UserId, entry.EntryDate }).IsUnique();
            entity.HasOne(entry => entry.User)
                .WithMany()
                .HasForeignKey(entry => entry.UserId);
        });
    }
}
