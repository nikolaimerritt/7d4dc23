using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTFWhodunnit.Models;
using CTFWhodunnit.Utils;
using Microsoft.EntityFrameworkCore;
using PirateConquest.Models;
using PirateConquest.ViewModels;
using SQLitePCL;

namespace CTFWhodunnit.Database;

public class DatabaseInitialiser
{
    public static readonly int DEFAULT_MAX_CTF_POINTS = 100;
    public static readonly int DEFAULT_MAX_FLAG_POINTS = 100;
    public static readonly int StartingShips = 10;

    private readonly AppDbContext _context;

    private static readonly int RoundCount = 5;
    private static readonly TimeSpan RoundDuration = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan RoundMovingDuration = TimeSpan.FromMinutes(29);
    private static readonly DateTime FirstRoundStart = DateTime.UtcNow + TimeSpan.FromSeconds(5);

    public DatabaseInitialiser(AppDbContext context)
    {
        _context = context;
    }

    public async Task Initialise()
    {
        _context.Database.EnsureCreated();

        if (!_context.AppConfigs.Any())
        {
            await InitializeConfig();
        }

        if (!await _context.Seas.AnyAsync())
        {
            await CreateSeas();
        }

        if (!await _context.AdjacentSeas.AnyAsync())
        {
            await CreateAdjacentSeas();
        }

        if (!await _context.Teams.AnyAsync())
        {
            await CreateTeams();
        }

        if (!await _context.Outcomes.AnyAsync())
        {
            await CreateInitialOutcome();
        }

        if (!await _context.Rounds.Where(round => !round.IsInitial).AnyAsync())
        {
            await CreateRounds();
        }
    }

    private async Task CreateSeas()
    {
        await _context.Seas.AddRangeAsync(Sea.Names.All.Select(name => new Sea() { Name = name }));
        await _context.SaveChangesAsync();
    }

    private async Task CreateAdjacentSeas()
    {
        foreach (var (sea, adjacentSeas) in Sea.Names.AdjacentSeas.AsEnumerable())
        {
            var seaEntry = await _context.Seas.FirstOrDefaultAsync(seaEntry =>
                seaEntry.Name == sea
            );
            var adjacentSeaEntries = await Task.WhenAll(
                adjacentSeas.Select(
                    async (adjacentSea) =>
                        new AdjacentSea()
                        {
                            Sea = seaEntry,
                            AdjacentTo = await _context.Seas.FirstOrDefaultAsync(sea =>
                                sea.Name == adjacentSea
                            )
                        }
                )
            );

            await _context.AdjacentSeas.AddRangeAsync(adjacentSeaEntries);
        }

        await _context.SaveChangesAsync();
    }

    private async Task CreateTeams()
    {
        var teams = new List<Team>()
        {
            new()
            {
                Name = "Team Drake",
                PlainTextPassword = "AwardTrafficSteeple",
                ColourHexCode = "#FAECDB",
                StartingSea = await _context.Seas.FirstOrDefaultAsync(sea =>
                    sea.Name == Sea.Names.NorthPacific
                )
            },
            new()
            {
                Name = "Team Read",
                PlainTextPassword = "NorthBesiegeSpine",
                ColourHexCode = "#C9E4DE",
                StartingSea = await _context.Seas.FirstOrDefaultAsync(sea =>
                    sea.Name == Sea.Names.SouthPacific
                )
            },
            new()
            {
                Name = "Team Kidd",
                PlainTextPassword = "UnequalGoodbyePossess",
                ColourHexCode = "#C6DEF1",
                StartingSea = await _context.Seas.FirstOrDefaultAsync(sea =>
                    sea.Name == Sea.Names.NorthAtlantic
                )
            },
            new()
            {
                Name = "Team Blackbeard",
                PlainTextPassword = "SpeakBraveWelcome",
                ColourHexCode = "#DBCDF0",
                StartingSea = await _context.Seas.FirstOrDefaultAsync(sea =>
                    sea.Name == Sea.Names.SouthAtlantic
                )
            },
            new()
            {
                Name = "Team O'Malley",
                PlainTextPassword = "ShallotSupposePreach",
                ColourHexCode = "#F7D9C4",
                StartingSea = await _context.Seas.FirstOrDefaultAsync(sea =>
                    sea.Name == Sea.Names.Southern
                )
            }
        };
        await _context.AddRangeAsync(teams);
        await _context.SaveChangesAsync();
    }

    private async Task CreateInitialOutcome()
    {
        var past = DateTime.UtcNow - TimeSpan.FromDays(1);
        var initialRound = new Round()
        {
            IsInitial = true,
            StartMoving = past,
            StartFighting = past,
            End = past,
        };
        await _context.Rounds.AddAsync(initialRound);
        var teams = await _context.Teams.Include(team => team.StartingSea).ToListAsync();
        var initialOutcomes = teams.Select(team => new Outcome()
        {
            Round = initialRound,
            Team = team,
            Sea = team.StartingSea,
            ShipCount = StartingShips
        });
        await _context.Outcomes.AddRangeAsync(initialOutcomes);
        await _context.SaveChangesAsync();
    }

    private async Task CreateRounds()
    {
        var lastRound = new Round()
        {
            StartMoving = FirstRoundStart,
            StartFighting = FirstRoundStart + RoundMovingDuration,
            End = FirstRoundStart + RoundDuration
        };
        await _context.Rounds.AddAsync(lastRound);
        for (var n = 1; n < RoundCount; n++)
        {
            var roundStart = lastRound.End + TimeSpan.FromSeconds(1);
            lastRound = new Round()
            {
                StartMoving = roundStart,
                StartFighting = roundStart + RoundMovingDuration,
                End = roundStart + RoundDuration,
            };
            await _context.Rounds.AddAsync(lastRound);
        }
        await _context.SaveChangesAsync();
    }

    private async Task InitializeConfig()
    {
        var conf = new AppConfig { Name = AppConfig.CTF_ID_KEY, Value = "93" };
        _context.AppConfigs.Add(conf);

        conf = new AppConfig { Name = AppConfig.TEAM_VIEW_KEY, Value = "false" };
        _context.AppConfigs.Add(conf);

        conf = new AppConfig
        {
            Name = AppConfig.MAX_FLAG_POINTS_KEY,
            Value = DEFAULT_MAX_FLAG_POINTS.ToString()
        };
        _context.AppConfigs.Add(conf);

        conf = new AppConfig
        {
            Name = AppConfig.MAX_CTF_POINTS_KEY,
            Value = DEFAULT_MAX_CTF_POINTS.ToString()
        };
        _context.AppConfigs.Add(conf);

        conf = new AppConfig
        {
            Name = AppConfig.PLAYGROUND_LEADERBOARD_URL_KEY,
            Value = "https://playground.withsecure.com/api/public/ctf/$ctfEventId/leaderboard"
        };
        _context.AppConfigs.Add(conf);

        conf = new AppConfig
        {
            Name = AppConfig.VIDEO_URL_KEY,
            Value =
                "https://pgmediauk.blob.core.windows.net/ctfs/Video-v2.mp4?sp=r&st=2023-09-05T16:41:41Z&se=2023-11-10T01:41:41Z&spr=https&sv=2022-11-02&sr=b&sig=egD4oauVLJmnscbwzcaRcxrZfmagL%2BQR080V%2FQz44gA%3D"
        };
        _context.AppConfigs.Add(conf);
        await _context.SaveChangesAsync();
    }
}
