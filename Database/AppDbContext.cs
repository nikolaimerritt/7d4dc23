using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTFWhodunnit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PirateConquest.Models;

namespace CTFWhodunnit.Database;

public class AppDbContext : DbContext
{
    public DbSet<AppConfig> AppConfigs { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Sea> Seas { get; set; }
    public DbSet<AdjacentSea> AdjacentSeas { get; set; }
    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<Round> Rounds { get; set; }
    public DbSet<Move> Moves { get; set; }
    public DbSet<Outcome> Outcomes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=ctfchallenge.db;");
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
        );

        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? v.Value.ToUniversalTime() : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v
        );

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (entityType.IsKeyless)
            {
                continue;
            }

            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(nullableDateTimeConverter);
                }
            }
        }
    }
}
