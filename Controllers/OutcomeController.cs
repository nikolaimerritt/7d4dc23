using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PirateConquest.Database;
using PirateConquest.Models;
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
            .OrderByDescending(outcome => outcome.Round.StartPlanning)
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
            (await _outcomeRepository.InPreviousRoundAsync()).Select(OutcomeViewModel.FromModel)
        );
    }

    //[HttpGet("/api/outcomes/state/current")]
    //public async Task<IActionResult> GetLatestVirtualOutcomes()
    //{
    //    var round = await _roundRepository.GetCurrentRoundAsync();
    //    var outcomes = await _outcomeRepository.InPreviousRoundAsync();
    //    var currentState = outcomes.Select(OutcomeViewModel.FromModel).ToList();

    //    foreach (var team in await _teamRepository.AllAsync())
    //    {
    //        await AddCurrentPurchases(round, team, currentState);
    //        await AddCurrentMoves(round, team, currentState);
    //    }

    //    return Json(currentState);
    //}

    //private async Task AddCurrentPurchases(Round round, Team team, List<OutcomeViewModel> currentState)
    //{
    //    var purchases = await _purchaseRepository.TeamPurchasesAsync(round, team);
    //    foreach (var purchase in purchases)
    //    {
    //        var outcomeInPurchaseDestination = currentState.FirstOrDefault(outcome =>
    //            outcome.Team.Id == team.Id && outcome.Sea.Id == purchase.Sea.Id
    //        );
    //        if (outcomeInPurchaseDestination is null)
    //        {
    //            currentState.Add(
    //                OutcomeViewModel.FromModel(
    //                    new()
    //                    {
    //                        Id = -1,
    //                        Round = round,
    //                        Team = team,
    //                        Sea = purchase.Sea,
    //                        ShipsAfter = purchase.ShipCount,
    //                    }
    //                )
    //            );
    //        }
    //        else
    //        {
    //            outcomeInPurchaseDestination.ShipsAfter += purchase.ShipCount;
    //        }
    //    }
    //}

    //private async Task AddCurrentMoves(Round round, Team team, List<OutcomeViewModel> currentState)
    //{
    //    var moves = (await _moveRepository.AllAsync())
    //        .Where(move => move.Round.Id == round.Id && move.Team.Id == team.Id)
    //        .ToList();

    //    foreach (var move in moves)
    //    {
    //        var outcomeInExitSea = currentState.FirstOrDefault(outcome =>
    //            outcome.Team.Id == team.Id && outcome.Sea.Id == move.FromSea.Id
    //        );
    //        if (outcomeInExitSea is not null)
    //        {
    //            outcomeInExitSea.ShipsAfter -= move.ShipCount;
    //        }
    //        var outcomeInEntranceSea = currentState.FirstOrDefault(outcome =>
    //            outcome.Team.Id == team.Id && outcome.Sea.Id == move.ToSea.Id
    //        );
    //        if (outcomeInEntranceSea is null)
    //        {
    //            currentState.Add(
    //                OutcomeViewModel.FromModel(
    //                    new()
    //                    {
    //                        Id = -1,
    //                        Round = round,
    //                        Team = team,
    //                        Sea = move.ToSea,
    //                        ShipsAfter = move.ShipCount,
    //                    }
    //                )
    //            );
    //        }
    //        else
    //        {
    //            outcomeInEntranceSea.ShipsAfter += move.ShipCount;
    //        }
    //    }
    //}

    [HttpGet("/api/outcomes/{outcomeId}")]
    public async Task<IActionResult> GetOutcome(int? outcomeId)
    {
        var outcome = (await _outcomeRepository.AllAsync()).Find(outcome =>
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
