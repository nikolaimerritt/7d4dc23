using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTFWhodunnit.Models;
using CTFWhodunnit.Utils;
using Microsoft.EntityFrameworkCore;
using PirateConquest.Models;

namespace CTFWhodunnit.Database;

public static class DbInitializer
{
    public static readonly int DEFAULT_MAX_CTF_POINTS = 100;
    public static readonly int DEFAULT_MAX_FLAG_POINTS = 100;
    public static readonly int STARTING_SHIPS = 10;

    public static async Task Initialize(AppDbContext context)
    {
        context.Database.EnsureCreated();

        if (!context.AppConfigs.Any())
        {
            await InitializeConfig(context);
        }

        if (!context.UnlockedIntels.Any())
        {
            Initializers.UnlockSuspectsByTeam(context);
        }

        if (!await context.Seas.AnyAsync())
        {
            await CreateSeas(context);
        }

        if (!await context.AdjacentSeas.AnyAsync())
        {
            await CreateAdjacentSeas(context);
        }

        if (!await context.Teams.AnyAsync())
        {
            await CreateTeams(context);
        }

        if (!await context.Outcomes.AnyAsync())
        {
            await CreateInitialOutcome(context);
        }
    }

    private static async Task InitializeConfig(AppDbContext context)
    {
        var conf = new AppConfig { Name = AppConfig.CTF_ID_KEY, Value = "93" };
        context.AppConfigs.Add(conf);

        conf = new AppConfig { Name = AppConfig.TEAM_VIEW_KEY, Value = "false" };
        context.AppConfigs.Add(conf);

        conf = new AppConfig
        {
            Name = AppConfig.MAX_FLAG_POINTS_KEY,
            Value = DEFAULT_MAX_FLAG_POINTS.ToString()
        };
        context.AppConfigs.Add(conf);

        conf = new AppConfig
        {
            Name = AppConfig.MAX_CTF_POINTS_KEY,
            Value = DEFAULT_MAX_CTF_POINTS.ToString()
        };
        context.AppConfigs.Add(conf);

        conf = new AppConfig
        {
            Name = AppConfig.PLAYGROUND_LEADERBOARD_URL_KEY,
            Value = "https://playground.withsecure.com/api/public/ctf/$ctfEventId/leaderboard"
        };
        context.AppConfigs.Add(conf);

        conf = new AppConfig
        {
            Name = AppConfig.VIDEO_URL_KEY,
            Value =
                "https://pgmediauk.blob.core.windows.net/ctfs/Video-v2.mp4?sp=r&st=2023-09-05T16:41:41Z&se=2023-11-10T01:41:41Z&spr=https&sv=2022-11-02&sr=b&sig=egD4oauVLJmnscbwzcaRcxrZfmagL%2BQR080V%2FQz44gA%3D"
        };
        context.AppConfigs.Add(conf);
        await context.SaveChangesAsync();
    }

    private static async Task CreateSeas(AppDbContext context)
    {
        await context.Seas.AddRangeAsync(Sea.Names.All.Select(name => new Sea() { Name = name }));
        await context.SaveChangesAsync();
    }

    private static async Task CreateAdjacentSeas(AppDbContext context)
    {
        foreach (var (sea, adjacentSeas) in Sea.Names.AdjacentSeas.AsEnumerable())
        {
            var seaEntry = await context.Seas.FirstOrDefaultAsync(seaEntry => seaEntry.Name == sea);
            var adjacentSeaEntries = await Task.WhenAll(
                adjacentSeas.Select(
                    async (adjacentSea) =>
                        new AdjacentSea()
                        {
                            Sea = seaEntry,
                            AdjacentTo = await context.Seas.FirstOrDefaultAsync(sea =>
                                sea.Name == adjacentSea
                            )
                        }
                )
            );

            await context.AdjacentSeas.AddRangeAsync(adjacentSeaEntries);
        }

        await context.SaveChangesAsync();
    }

    private static async Task CreateTeams(AppDbContext context)
    {
        await context.Teams.AddRangeAsync(
            new Team()
            {
                Name = "TeamViewModel Drake",
                PlainTextPassword = "AwardTrafficSteeple",
                ColourHexCode = "#FAECDB",
                StartingSea = await context.Seas.FirstOrDefaultAsync(sea =>
                    sea.Name == Sea.Names.NorthPacific
                )
            },
            new Team()
            {
                Name = "TeamViewModel Morgan",
                PlainTextPassword = "NorthBesiegeSpine",
                ColourHexCode = "#C9E4DE",
                StartingSea = await context.Seas.FirstOrDefaultAsync(sea =>
                    sea.Name == Sea.Names.SouthPacific
                )
            },
            new Team()
            {
                Name = "TeamViewModel Kidd",
                PlainTextPassword = "UnequalGoodbyePossess",
                ColourHexCode = "#C6DEF1",
                StartingSea = await context.Seas.FirstOrDefaultAsync(sea =>
                    sea.Name == Sea.Names.NorthAtlantic
                )
            },
            new Team()
            {
                Name = "TeamViewModel Blackbeard",
                PlainTextPassword = "SpeakBraveWelcome",
                ColourHexCode = "#DBCDF0",
                StartingSea = await context.Seas.FirstOrDefaultAsync(sea =>
                    sea.Name == Sea.Names.SouthAtlantic
                )
            },
            new Team()
            {
                Name = "TeamViewModel Jack",
                PlainTextPassword = "ShallotSupposePreach",
                ColourHexCode = "#F7D9C4",
                StartingSea = await context.Seas.FirstOrDefaultAsync(sea =>
                    sea.Name == Sea.Names.Southern
                )
            }
        );
        await context.SaveChangesAsync();
    }

    private static async Task CreateInitialOutcome(AppDbContext context)
    {
        var now = DateTime.UtcNow;
        var initialRound = new Round()
        {
            StartMoving = now,
            StartFighting = now,
            End = now
        };
        var initialOutcomes = context.Teams.Select(team => new Outcome()
        {
            Round = initialRound,
            Team = team,
            Sea = team.StartingSea,
            ShipCount = STARTING_SHIPS
        });
        await context.Outcomes.AddRangeAsync(initialOutcomes);
        await context.SaveChangesAsync();
    }
}
