using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PirateConquest.Models;
using PirateConquest.Models;
using PirateConquest.Repositories;
using PirateConquest.Services;
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

    public DatabaseInitialiser(AppDbContext context)
    {
        _context = context;
    }

    public async Task Initialise(Configuration configuration)
    {
        _context.Database.EnsureCreated();

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
            await CreateRounds(configuration);
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
                StartingSea = await _context.Seas.FirstOrDefaultAsync(sea =>
                    sea.Name == Sea.Names.NorthPacific
                )
            },
            new()
            {
                Name = "Team Read",
                PlainTextPassword = "NorthBesiegeSpine",
                StartingSea = await _context.Seas.FirstOrDefaultAsync(sea =>
                    sea.Name == Sea.Names.SouthPacific
                )
            },
            new()
            {
                Name = "Team Kidd",
                PlainTextPassword = "UnequalGoodbyePossess",
                StartingSea = await _context.Seas.FirstOrDefaultAsync(sea =>
                    sea.Name == Sea.Names.NorthAtlantic
                )
            },
            new()
            {
                Name = "Team Blackbeard",
                PlainTextPassword = "SpeakBraveWelcome",
                StartingSea = await _context.Seas.FirstOrDefaultAsync(sea =>
                    sea.Name == Sea.Names.SouthAtlantic
                )
            },
            new()
            {
                Name = "Team O'Malley",
                PlainTextPassword = "ShallotSupposePreach",
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

    private async Task CreateRounds(Configuration configuration)
    {
        var planningTime = TimeSpan.FromMinutes(configuration.PlanningMinutes);
        var cooldownTime = TimeSpan.FromMinutes(configuration.CooldownMinutes);
        var lastRound = new Round()
        {
            StartPlanning = configuration.FirstRoundStartUtc,
            StartCooldown = configuration.FirstRoundStartUtc + planningTime,
            End = configuration.FirstRoundStartUtc + planningTime + cooldownTime
        };
        await _context.Rounds.AddAsync(lastRound);
        for (var n = 1; n < configuration.RoundsCount; n++)
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
}
