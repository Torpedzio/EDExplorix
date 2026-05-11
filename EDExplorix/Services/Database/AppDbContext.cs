using System;
using System.IO;
using EDExplorix.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace EDExplorix.Services.Database;

public class AppDbContext : DbContext
{
    public DbSet<StarSystem> StarSystems { get; set; }
    public DbSet<Body> Bodies { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "EDExplorix",
            "explorix.db"
        );

        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StarSystem>(entity =>
        {
            entity.HasKey(e => e.SystemAddress);
            entity.Property(e => e.Name).IsRequired();

            entity.HasMany(e => e.Bodies)
                .WithOne(e => e.StarSystem)
                .HasForeignKey(e => e.SystemAddress)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Body>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.SystemAddress, e.BodyId }).IsUnique();
        });
    }
}