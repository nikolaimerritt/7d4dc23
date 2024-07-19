using CTFWhodunnit.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PirateConquest.ViewModels;

namespace PirateConquest.Controllers;

public class OutcomeController : Controller
{
    private readonly AppDbContext _context;

    public OutcomeController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("/api/outcomes/{outcomeId}")]
    public async Task<IActionResult> GetOutcomes(int? outcomeId)
    {
        if (outcomeId is int id)
        {
            var outcome = await _context.Outcomes.FirstOrDefaultAsync(outcome => outcome.Id == id);
            if (outcome is null)
            {
                return NotFound();
            }
            else
            {
                return Json(outcome);
            }
        }
        else
        {
            var outcomes = await _context
                .Outcomes.OrderByDescending(outcome => outcome.Round.StartMoving)
                .ThenBy(outcome => outcome.Id)
                .ToListAsync();
            return Json(outcomes);
        }
    }

    [HttpGet("/api/outcomes/latest")]
    public async Task<IActionResult> GetOutcomesFromLatestRound()
    {
        var latestRound = await _context
            .Rounds.OrderByDescending(round => round.StartMoving)
            .FirstOrDefaultAsync();
        var latestOutcomes = _context
            .Outcomes.Where(outcome => outcome.Round == latestRound)
            .OrderBy(outcome => outcome.Id)
            .ToListAsync();

        return Json(latestOutcomes);
    }
}
