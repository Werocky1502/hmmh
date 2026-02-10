using Hmmh.Api.Models;
using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore;

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
        // Forward configuration to the base IdentityDbContext.
    }

    /// <summary>
    ///     Users stored locally for authentication.
    /// </summary>
    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();

    /// <summary>
    ///     Weight entries recorded by users.
    /// </summary>
    public DbSet<WeightEntry> WeightEntries => Set<WeightEntry>();

    /// <summary>
    ///     Calorie entries recorded by users.
    /// </summary>
    public DbSet<CalorieEntry> CalorieEntries => Set<CalorieEntry>();

    /// <summary>
    ///     Configures EF Core model mappings.
    /// </summary>
    /// <param name="modelBuilder">Model builder for entity configuration.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Register OpenIddict entities for OAuth/OIDC data.
        modelBuilder.UseOpenIddict();

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            // Store users in a dedicated table with unique logins.
            entity.ToTable("Users");
            entity.HasKey(user => user.Id);
            entity.Property(user => user.Id).HasColumnType("uuid");
            entity.Property(user => user.UserName).IsRequired();
            entity.Property(user => user.PasswordHash).IsRequired();
            entity.HasIndex(user => user.UserName).IsUnique();
        });

        modelBuilder.Entity<WeightEntry>(entity =>
        {
            // Enforce one weight record per user per date with consistent precision.
            entity.HasKey(entry => entry.Id);
            entity.Property(entry => entry.UserId).HasColumnType("uuid");
            entity.Property(entry => entry.EntryDate).HasColumnType("date");
            entity.Property(entry => entry.WeightKg).HasPrecision(5, 2);
            entity.HasIndex(entry => new { entry.UserId, entry.EntryDate }).IsUnique();
            entity.HasOne(entry => entry.User)
                .WithMany()
                .HasForeignKey(entry => entry.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CalorieEntry>(entity =>
        {
            // Allow multiple calorie records per user per date.
            entity.HasKey(entry => entry.Id);
            entity.Property(entry => entry.UserId).HasColumnType("uuid");
            entity.Property(entry => entry.EntryDate).HasColumnType("date");
            entity.HasIndex(entry => new { entry.UserId, entry.EntryDate });
            entity.HasOne(entry => entry.User)
                .WithMany()
                .HasForeignKey(entry => entry.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
