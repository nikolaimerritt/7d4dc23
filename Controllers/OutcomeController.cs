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
