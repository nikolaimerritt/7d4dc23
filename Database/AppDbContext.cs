using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PirateConquest.Models;
using PirateConquest.Models;

namespace PirateConquest.Database;

public class AppDbContext : DbContext
{
    public DbSet<Team> Teams { get; set; }
    public DbSet<Sea> Seas { get; set; }
    public DbSet<AdjacentSea> AdjacentSeas { get; set; }
    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<Round> Rounds { get; set; }
    public DbSet<Move> Moves { get; set; }
    public DbSet<Outcome> Outcomes { get; set; }
    public DbSet<Configuration> Configurations { get; set; }

    public DbSet<Message> Messages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=ctfchallenge.db;");
    }

    // SQLite does not store DateTime timezones.
    // This is a workaround that reads all datetimes from the database as UTC.
    // All datetimes written to the database are assumed to be in UTC.
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
