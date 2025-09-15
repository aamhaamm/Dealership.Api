using Dealership.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Dealership.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<OtpToken> OtpTokens => Set<OtpToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Email must be unique
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        // Composite index for OTP purpose + identifier
        modelBuilder.Entity<OtpToken>().HasIndex(o => new { o.Purpose, o.Identifier });

        base.OnModelCreating(modelBuilder);
    }
}
