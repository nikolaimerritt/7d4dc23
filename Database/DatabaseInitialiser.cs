using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PirateConquest.Models;
using PirateConquest.Models;
using PirateConquest.Repositories;
using PirateConquest.Utils;
using PirateConquest.ViewModels;
using SQLitePCL;

namespace PirateConquest.Database;

public class DatabaseInitialiser
{
    public static readonly int DEFAULT_MAX_CTF_POINTS = 100;
    public static readonly int DEFAULT_MAX_FLAG_POINTS = 100;
    public static readonly int StartingShips = 10;

    private readonly AppDbContext _context;
    private readonly ConfigService _configRepository;

    public DatabaseInitialiser(AppDbContext context, ConfigService configRepository)
    {
        _context = context;
        _configRepository = configRepository;
    }

    public async Task Initialise()
    {
        _context.Database.EnsureCreated();

        if (!await _context.AppConfigs.AnyAsync())
        {
            await InitialiseConfig();
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
            StartPlanning = past,
            StartCooldown = past,
            End = past,
        };
        await _context.Rounds.AddAsync(initialRound);
        var teams = await _context.Teams.Include(team => team.StartingSea).ToListAsync();
        var initialOutcomes = teams.Select(team => new Outcome()
        {
            Round = initialRound,
            Team = team,
            Sea = team.StartingSea,
            ShipsAfter = StartingShips
        });
        await _context.Outcomes.AddRangeAsync(initialOutcomes);
        await _context.SaveChangesAsync();
    }

    private async Task CreateRounds()
    {
        var firstRoundStart =
            await _configRepository.GetValueAsync(AppConfig.DateTimeConfig.FirstRoundStart)
            ?? throw new ArgumentNullException(
                $"Could not find an environment variable or database configuration entry for the time when the first round starts. Searched for key {AppConfig.DateTimeConfig.FirstRoundStart.Key}"
            );
        var planningMinutes =
            await _configRepository.GetValueAsync(AppConfig.IntegerConfig.PlanningMinutes)
            ?? throw new ArgumentNullException(
                $"Could not find an environment variable or database configuration entry for the amount of planning minutes. Searched for key {AppConfig.IntegerConfig.PlanningMinutes.Key}"
            );
        var cooldownMinutes =
            await _configRepository.GetValueAsync(AppConfig.IntegerConfig.CooldownMinutes)
            ?? throw new ArgumentNullException(
                $"Could not find an environment variable or database configuration entry for the amount of cooldown minutes. Searched for key {AppConfig.IntegerConfig.CooldownMinutes.Key}"
            );
        var roundCount =
            await _configRepository.GetValueAsync(AppConfig.IntegerConfig.RoundsCount)
            ?? throw new ArgumentNullException(
                $"Could not find an environment variable or database configuration entry for the number of rounds. Searched for key {AppConfig.IntegerConfig.RoundsCount.Key}"
            );

        var planningTime = TimeSpan.FromMinutes(planningMinutes);
        var cooldownTime = TimeSpan.FromMinutes(cooldownMinutes);
        var lastRound = new Round()
        {
            StartPlanning = firstRoundStart,
            StartCooldown = firstRoundStart + planningTime,
            End = firstRoundStart + planningTime + cooldownTime
        };
        await _context.Rounds.AddAsync(lastRound);
        for (var n = 1; n < roundCount; n++)
        {
            var roundStart = lastRound.End + TimeSpan.FromSeconds(1);
            lastRound = new Round()
            {
                StartPlanning = roundStart,
                StartCooldown = roundStart + planningTime,
                End = roundStart + planningTime + cooldownTime,
            };
            await _context.Rounds.AddAsync(lastRound);
        }
        await _context.SaveChangesAsync();
    }

    private async Task InitialiseConfig()
    {
        var configs = new List<AppConfig>()
        {
            new() { Key = AppConfig.IntegerConfig.CtfIdKey.Key, Value = "94" },
            new()
            {
                Key = AppConfig.StringConfig.PlaygroundLeaderboardUrl.Key,
                Value = "https://playground.withsecure.com/api/public/ctf/$ctfEventId/leaderboard"
            },
        };
        await _context.AppConfigs.AddRangeAsync(configs);
        await _context.SaveChangesAsync();
    }
}
