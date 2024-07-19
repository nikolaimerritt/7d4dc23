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
    public DbSet<AppConfig> AppConfigs { get; set; }

    // New
    public DbSet<Team> Teams { get; set; }
    public DbSet<Sea> Seas { get; set; }
    public DbSet<AdjacentSea> AdjacentSeas { get; set; }
    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<Round> Rounds { get; set; }
    public DbSet<Move> Moves { get; set; }
    public DbSet<Outcome> Outcomes { get; set; }

    public async Task<Round?> GetMovingRoundAsync()
    {
        var now = DateTime.UtcNow;
        return await Rounds.FirstOrDefaultAsync(round =>
            round.StartMoving <= now && now < round.StartFighting
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=ctfchallenge.db");
    }
}
