using CTFWhodunnit.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PirateConquest.Repositories;
using PirateConquest.Utils;
using PirateConquest.ViewModels;
using SQLitePCL;

namespace PirateConquest.Controllers;

public class OutcomeController : Controller
{
    private readonly OutcomeRepository _outcomeRepository;
    private readonly TeamRepository _teamRepository;
    private readonly MoveRepository _moveRepository;
    private readonly RoundRepository _roundRepository;
    private readonly PurchaseRepository _purchaseRepository;

    public OutcomeController(
        OutcomeRepository outcomeRepository,
        TeamRepository teamRepository,
        MoveRepository moveRepository,
        RoundRepository roundRepository,
        PurchaseRepository purchaseRepository
    )
    {
        _outcomeRepository = outcomeRepository;
        _teamRepository = teamRepository;
        _moveRepository = moveRepository;
        _roundRepository = roundRepository;
        _purchaseRepository = purchaseRepository;
    }

    [HttpGet("/api/outcomes")]
    public async Task<IActionResult> GetOutcomes(int? roundId = null)
    {
        var outcomes = (await _outcomeRepository.AllAsync())
            .OrderByDescending(outcome => outcome.Round.StartMoving)
            .ThenBy(outcome => outcome.Id)
            .ToList();
        if (roundId is int id)
        {
            outcomes = outcomes.Where(outcome => outcome.RoundId == id).ToList();
        }
        return Json(outcomes.Select(OutcomeViewModel.FromModel));
    }

    [HttpGet("/api/outcomes/latest")]
    public async Task<IActionResult> GetLatestOutcomes()
    {
        return Json(
            (await _outcomeRepository.FromPreviousRoundAsync()).Select(OutcomeViewModel.FromModel)
        );
    }

    [HttpGet("/api/outcomes/virtual/latest")]
    public async Task<IActionResult> GetLatestVirtualOutcomes()
    {
        var team = await _teamRepository.ByIdAsync(User.GetTeamId());
        if (team is null)
        {
            return Json(ErrorViewModel.Unauthorized);
        }

        var round = await _roundRepository.GetCurrentRoundAsync();
        var outcomes = await _outcomeRepository.FromPreviousRoundAsync();
        var virtualOutcomes = outcomes.Select(OutcomeViewModel.FromModel).ToList();

        var moves = (await _moveRepository.All())
            .Where(move => move.Round.Id == round.Id && move.Team.Id == team.Id)
            .ToList();
        var purchases = await _purchaseRepository.TeamPurchasesAsync(round, team);
        foreach (var purchase in purchases)
        {
            var outcomeInPurchaseDestination = virtualOutcomes.FirstOrDefault(outcome =>
                outcome.Team.Id == team.Id && outcome.Sea.Id == purchase.Sea.Id
            );
            if (outcomeInPurchaseDestination is null)
            {
                virtualOutcomes.Add(
                    OutcomeViewModel.FromModel(
                        new()
                        {
                            Id = -1,
                            Round = round,
                            Team = team,
                            Sea = purchase.Sea,
                            ShipCount = purchase.ShipCount,
                        }
                    )
                );
            }
            else
            {
                outcomeInPurchaseDestination.ShipCount += purchase.ShipCount;
            }
        }

        foreach (var move in moves)
        {
            var outcomeInExitSea = virtualOutcomes.FirstOrDefault(outcome =>
                outcome.Team.Id == team.Id && outcome.Sea.Id == move.FromSea.Id
            );
            if (outcomeInExitSea is not null)
            {
                outcomeInExitSea.ShipCount -= move.ShipCount;
            }
            var outcomeInEntranceSea = virtualOutcomes.FirstOrDefault(outcome =>
                outcome.Team.Id == team.Id && outcome.Sea.Id == move.ToSea.Id
            );
            if (outcomeInEntranceSea is null)
            {
                virtualOutcomes.Add(
                    OutcomeViewModel.FromModel(
                        new()
                        {
                            Id = -1,
                            Round = round,
                            Team = team,
                            Sea = move.ToSea,
                            ShipCount = move.ShipCount,
                        }
                    )
                );
            }
            else
            {
                outcomeInEntranceSea.ShipCount += move.ShipCount;
            }
        }

        return Json(virtualOutcomes);
    }

    [HttpGet("/api/outcomes/{outcomeId}")]
    public async Task<IActionResult> GetOutcome(int? outcomeId)
    {
        var outcome = (await _outcomeRepository.AllAsync()).FirstOrDefault(outcome =>
            outcome.Id == outcomeId
        );
        if (outcome is null)
        {
            return NotFound();
        }
        else
        {
            return Json(OutcomeViewModel.FromModel(outcome));
        }
    }
}
