using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTFWhodunnit.Models;
using Microsoft.EntityFrameworkCore;

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


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=ctfchallenge.db");
    }
}
