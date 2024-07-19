using CTFWhodunnit.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PirateConquest.Repositories;
using PirateConquest.ViewModels;

namespace PirateConquest.Controllers;

public class OutcomeController : Controller
{
    private readonly AppDbContext _context;
    private readonly OutcomeRepository _outcomeRepository;

    public OutcomeController(AppDbContext context, OutcomeRepository outcomeRepository)
    {
        _context = context;
        _outcomeRepository = outcomeRepository;
    }

    [HttpGet("/api/outcomes")]
    public async Task<IActionResult> GetAllOutcomes()
    {
        var outcomes = (await _outcomeRepository.All())
            .OrderByDescending(outcome => outcome.Round.StartMoving)
            .ThenBy(outcome => outcome.Id);
        return Json(outcomes);
    }

    [HttpGet("/api/outcomes/{outcomeId}")]
    public async Task<IActionResult> GetOutcome(int? outcomeId)
    {
        var outcome = (await _outcomeRepository.All()).FirstOrDefault(outcome =>
            outcome.Id == outcomeId
        );
        if (outcome is null)
        {
            return NotFound();
        }
        else
        {
            return Json(outcome);
        }
    }

    [HttpGet("/api/outcomes/latest")]
    public async Task<IActionResult> GetLatestOutcomes()
    {
        return Json(await _outcomeRepository.LatestOutcomes());
    }
}
