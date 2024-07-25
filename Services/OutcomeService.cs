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
        var roundHasOutcomes = (await _outcomeRepository.AllAsync())
            .Where(outcome => outcome.Round.Id == round.Id)
            .Any();
        if (!roundHasOutcomes)
        {
            var teams = await _teamRepository.AllAsync();
            var seas = await _seaRepository.AllAsync();

            foreach (var sea in seas)
            {
                var teamsWithShips = await Task.WhenAll(
                    teams.Select(async team =>
                    {
                        var shipCount = await _roundRepository.CountTeamShipsAsync(
                            sea,
                            team,
                            round
                        );
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
                    await _outcomeRepository.AddIfNotExists(
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

                    foreach (var team in losingTeams)
                    {
                        await _outcomeRepository.AddIfNotExists(
                            new Outcome()
                            {
                                RoundId = round.Id,
                                SeaId = sea.Id,
                                TeamId = team.Team.Id,
                                ShipCount = 0
                            }
                        );
                    }
                }
            }
        }
    }
}
