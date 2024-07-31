namespace PirateConquest.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using PirateConquest.Models;
using PirateConquest.Repositories;
using PirateConquest.ViewModels;

public class LeaderboardController : Controller
{
    private readonly TeamRepository _teamRepository;
    private readonly RoundRepository _roundRepository;
    private readonly OutcomeRepository _outcomeRepository;
    private readonly SeaRepository _seaRepository;

    public LeaderboardController(
        TeamRepository teamRepository,
        RoundRepository roundRepository,
        OutcomeRepository outcomeRepository,
        SeaRepository seaRepository
    )
    {
        _teamRepository = teamRepository;
        _roundRepository = roundRepository;
        _outcomeRepository = outcomeRepository;
        _seaRepository = seaRepository;
    }

    [HttpGet("/api/leaderboard")]
    public async Task<IActionResult> GetLeaderboard()
    {
        var seas = await _seaRepository.AllAsync();
        var teams = await _teamRepository.AllAsync();
        var leaderboardEntries = teams
            .Select(team => new LeaderboardEntryViewModel()
            {
                Team = TeamViewModel.FromModel(team),
                Rank = 0,
                SeasHeld = 0
            })
            .ToList();
        foreach (var round in await _roundRepository.AllPlayableRoundsAsync())
        {
            foreach (var sea in seas)
            {
                var outcomesInSea = await _outcomeRepository.WithinAsync(round, sea);
                var winningOutcome = outcomesInSea
                    .OrderByDescending(outcome => outcome.ShipsAfter)
                    .Where(outcome => outcome.ShipsAfter > 0)
                    .FirstOrDefault();
                if (winningOutcome is not null)
                {
                    var entry = leaderboardEntries.FirstOrDefault(entry =>
                        entry.Team.Id == winningOutcome.TeamId
                    );
                    if (entry is not null)
                    {
                        entry.SeasHeld++;
                    }
                }
            }
        }

        // Giving the same rank to teams that tie
        var groupedBySeasHeld = leaderboardEntries
            .GroupBy(entry => entry.SeasHeld)
            .OrderByDescending(group => group.Key)
            .ToList();
        var rank = 1;
        foreach (var group in groupedBySeasHeld)
        {
            foreach (var entry in group)
            {
                entry.Rank = rank;
            }
            rank += group.Count();
        }

        var entriesRanked = groupedBySeasHeld
            .SelectMany(group => group)
            .OrderBy(entry => entry.Rank)
            .ThenBy(entry => entry.Team.Name)
            .ToList();
        return Json(entriesRanked);
    }
}
