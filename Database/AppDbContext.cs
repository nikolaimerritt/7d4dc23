using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTFWhodunnit.Models;
using Microsoft.EntityFrameworkCore;
using PirateConquest.Models;

namespace CTFWhodunnit.Database;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Hint> Hints { get; set; }
    public DbSet<Guess> Guesses { get; set; }
    public DbSet<Suspect> Suspects { get; set; }
    public DbSet<Flag> Flags { get; set; }
    public DbSet<UnlockedIntel> UnlockedIntels { get; set; }
    public DbSet<AppConfig> AppConfigs { get; set; }

    // New
    public DbSet<Team> Teams { get; set; }
    public DbSet<Sea> Seas { get; set; }
    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<Round> Rounds { get; set; }
    public DbSet<Move> Moves { get; set; }
    public DbSet<Outcome> Outcomes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=ctfchallenge.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Team>().ToTable("Teams");
    }
}
