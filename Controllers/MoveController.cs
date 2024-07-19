using CTFWhodunnit.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PirateConquest.Utils;
using PirateConquest.ViewModels;

namespace PirateConquest.Controllers;

public class MoveController : Controller
{
    private readonly AppDbContext _context;

    public MoveController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("/api/moves/{moveId}")]
    public async Task<IActionResult> GetTeamMoves(int? moveId)
    {
        var team = await User.GetTeamAsync(_context);
        if (team is null)
        {
            return Unauthorized();
        }

        if (moveId is int id)
        {
            var move = await _context.Moves.FirstOrDefaultAsync(move => move.Id == id);
            if (move is null)
            {
                return NotFound();
            }
            else
            {
                return Json(move);
            }
        }
        else
        {
            var allTeamMoves = await _context.Moves.Where(move => move.Team == team).ToListAsync();
            return Json(allTeamMoves);
        }
    }

    [HttpPut("/api/moves")]
    public async Task<IActionResult> PutMove(int fromSeaId, int toSeaId, int shipCount)
    {
        var team = await User.GetTeamAsync(_context);
        if (team is null)
        {
            return BadRequest();
        }

        var fromSea = await _context.Seas.FirstOrDefaultAsync(sea => sea.Id == fromSeaId);
        var toSea = await _context.Seas.FirstOrDefaultAsync(sea => sea.Id == toSeaId);

        // TO SELF: check if toSea is accessible from fromSea
        if (fromSea is null || toSea is null)
        {
            return BadRequest();
        }

        var fromOutcome = await _context
            .Outcomes.Where(outcome => outcome.Team == team && outcome.Sea == fromSea)
            .OrderBy(outcome => outcome.Round.End)
            .FirstOrDefaultAsync();
        if (fromOutcome is null)
        {
            // TO SELF; figure out how to return server error
            return BadRequest();
        }
        if (shipCount < 0 || shipCount > fromOutcome?.ShipCount)
        {
            return BadRequest();
        }

        await _context.Moves.AddAsync(
            new()
            {
                Round = await _context.GetMovingRoundAsync(),
                Team = team,
                FromSea = fromSea,
                ToSea = toSea,
                ShipCount = shipCount,
                Creation = DateTime.UtcNow
            }
        );
        await _context.SaveChangesAsync();

        return Ok();
    }
}
