using System.Reflection;
using PirateConquest.Models;
using PirateConquest.Repositories;
using PirateConquest.Utils;

namespace PirateConquest.Services;

public class OutcomeService
{
    private static readonly double MaxBoostFactor = 1.1;
    private static readonly Random Random = new();
    private static readonly TimeSpan EndOfRoundWindow = TimeSpan.FromSeconds(15);
    private readonly TeamRepository _teamRepository;
    private readonly SeaRepository _seaRepository;
    private readonly OutcomeRepository _outcomeRepository;
    private readonly RoundRepository _roundRepository;

    public OutcomeService(
        TeamRepository teamRepository,
        SeaRepository seaRepository,
        OutcomeRepository outcomeRepository,
        RoundRepository roundRepository
    )
    {
        _teamRepository = teamRepository;
        _seaRepository = seaRepository;
        _outcomeRepository = outcomeRepository;
        _roundRepository = roundRepository;
    }

    public async Task WriteOutcomesAtEndOfRound(Round round)
    {
        var sleepDuration = TimeSpan.FromSeconds(0.5 * EndOfRoundWindow.TotalSeconds);
        while ((round.End - DateTime.UtcNow).TotalSeconds > EndOfRoundWindow.TotalSeconds)
        {
            Console.WriteLine(
                $"Waiting for round {round.Id} to end. Round ends in {round.End - DateTime.UtcNow}"
            );
            await Task.Delay(sleepDuration);
        }
        Console.WriteLine($"Round {round.Id} has ended. Writing outcomes.");
        await WriteOutcomes(round);
        Console.WriteLine($"Written outcomes for round {round.Id}.");
    }

    private async Task WriteOutcomes(Round round)
    {
        var roundHasOutcomes = (await _outcomeRepository.AllAsync())
            .Where(outcome => outcome.Round.Id == round.Id)
            .Any();
        if (roundHasOutcomes)
        {
            return;
        }
        var teams = await _teamRepository.AllAsync();
        var seas = await _seaRepository.AllAsync();

        foreach (var sea in seas)
        {
            var shipsCount = await GetTeamShipCounts(round, sea);
            if (shipsCount.Count > 0)
            {
                var winningTeam = shipsCount.FirstOrDefault();
                var losingTeams = shipsCount.Skip(1);
                if (losingTeams.Any())
                {
                    await _outcomeRepository.AddIfNotExists(
                        new Outcome()
                        {
                            RoundId = round.Id,
                            SeaId = sea.Id,
                            TeamId = winningTeam.Team.Id,
                            ShipsBefore = winningTeam.ShipCount,
                            ShipsAfter = Math.Max(
                                1,
                                winningTeam.BoostedShipCount
                                    - losingTeams.Sum(team => team.BoostedShipCount)
                            )
                        }
                    );

                    foreach (var team in losingTeams)
                    {
                        await _outcomeRepository.AddIfNotExists(
                            new Outcome()
                            {
                                RoundId = round.Id,
                                SeaId = sea.Id,
                                TeamId = team.Team.Id,
                                ShipsBefore = team.ShipCount,
                                ShipsAfter = 0
                            }
                        );
                    }
                }
                else
                {
                    await _outcomeRepository.AddIfNotExists(
                        new Outcome()
                        {
                            RoundId = round.Id,
                            SeaId = sea.Id,
                            TeamId = winningTeam.Team.Id,
                            ShipsBefore = winningTeam.ShipCount,
                            ShipsAfter = winningTeam.ShipCount
                        }
                    );
                }
            }
        }
    }

    private async Task<List<TeamShipCount>> GetTeamShipCounts(Round round, Sea sea)
    {
        var teams = await _teamRepository.AllAsync();
        var teamsWithShips = await Task.WhenAll(
            teams.Select(async team =>
            {
                var shipCount = await _roundRepository.CountTeamShipsAsync(sea, team, round);
                return new TeamShipCount
                {
                    Team = team,
                    ShipCount = shipCount,
                    BoostedShipCount = RandomBoost(shipCount)
                };
            })
        );
        var counts = teamsWithShips
            .Where(teamWithShips => teamWithShips.ShipCount > 0)
            .OrderByDescending(teamWithShips => teamWithShips.BoostedShipCount)
            .ToList();
        if (!counts.Any())
        {
            return counts;
        }

        var maxShips = counts[0].ShipCount;
        var winningCounts = counts.TakeWhile(count => count.ShipCount == maxShips);
        if (winningCounts.Count() > 1)
        {
            var tieBreakers = winningCounts.Select((_, index) => index).ToList().ShuffleInPlace();
            for (int i = 0; i < tieBreakers.Count; i++)
            {
                counts[i].BoostedShipCount += tieBreakers[i];
            }
        }
        return counts.OrderByDescending(count => count.BoostedShipCount).ToList();
    }

    private static int RandomBoost(int shipCount)
    {
        if (shipCount < 5)
        {
            return shipCount;
        }
        else if (shipCount <= Math.Ceiling(1.0 / (MaxBoostFactor - 1)))
        {
            return Random.Next(shipCount, shipCount + 1);
        }
        else
        {
            return Random.Next(shipCount, (int)Math.Round(shipCount * MaxBoostFactor));
        }
    }

    private class TeamShipCount
    {
        public Team Team { get; set; }
        public int ShipCount { get; set; }
        public int BoostedShipCount { get; set; }
    };
}
