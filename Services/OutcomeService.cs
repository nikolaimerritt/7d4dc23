using PirateConquest.Models;
using PirateConquest.Repositories;

namespace PirateConquest.Services;

public class OutcomeService
{
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

    public async Task WriteOutcomes(Round round)
    {
        var roundHasOutcomes = (await _outcomeRepository.All())
            .Where(outcome => outcome.Round.Id == round.Id)
            .Any();
        if (!roundHasOutcomes)
        {
            await _outcomeRepository.Add(await ComputeOutcomes(round));
        }
    }

    private async Task<List<Outcome>> ComputeOutcomes(Round round)
    {
        var teams = await _teamRepository.All();
        var seas = await _seaRepository.All();

        var outcomes = new List<Outcome>();
        foreach (var sea in seas)
        {
            var teamsWithShips = await Task.WhenAll(
                teams.Select(async team =>
                {
                    var shipCount = await _roundRepository.TeamShipCountAsync(round, sea, team);
                    return (Team: team, ShipCount: shipCount);
                })
            );
            var teamsRankedByShips = teamsWithShips
                .Where(teamWithShips => teamWithShips.ShipCount > 0)
                .OrderByDescending(teamWithShips => teamWithShips.ShipCount)
                .ToList();
            if (teamsRankedByShips.Count > 0)
            {
                var winningTeam = teamsRankedByShips.FirstOrDefault();
                var losingTeams = teamsRankedByShips.Skip(1);
                outcomes.Add(
                    new Outcome()
                    {
                        RoundId = round.Id,
                        SeaId = sea.Id,
                        TeamId = winningTeam.Team.Id,
                        ShipCount = Math.Max(
                            1,
                            winningTeam.ShipCount - losingTeams.Sum(team => team.ShipCount)
                        )
                    }
                );
                outcomes.AddRange(
                    losingTeams.Select(team => new Outcome()
                    {
                        RoundId = round.Id,
                        SeaId = sea.Id,
                        TeamId = team.Team.Id,
                        ShipCount = 0
                    })
                );
            }
        }
        return outcomes;
    }
}
