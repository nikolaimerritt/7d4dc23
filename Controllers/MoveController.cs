using CTFWhodunnit.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PirateConquest.Repositories;
using PirateConquest.Utils;
using PirateConquest.ViewModels;

namespace PirateConquest.Controllers;

public class MoveController : Controller
{
    private readonly AppDbContext _context;
    private readonly SeaRepository _seaRepository;
    private readonly MoveRepository _moveRepository;

    public MoveController(
        AppDbContext context,
        SeaRepository seaRepository,
        MoveRepository moveRepository
    )
    {
        _context = context;
        _seaRepository = seaRepository;
        _moveRepository = moveRepository;
    }

    [HttpGet("/api/moves")]
    public async Task<IActionResult> GetTeamMoves()
    {
        var team = await User.GetTeamAsync(_context);
        if (team is null)
        {
            return Unauthorized();
        }
        else
        {
            var allTeamMoves = (await _moveRepository.All())
                .Where(move => move.Team == team)
                .Select(MoveViewModel.FromModel);
            return Json(allTeamMoves);
        }
    }

    [HttpGet("/api/moves/{moveId}")]
    public async Task<IActionResult> GetTeamMove(int? moveId)
    {
        var team = await User.GetTeamAsync(_context);
        if (team is null)
        {
            return Unauthorized();
        }

        var move = (await _moveRepository.All()).FirstOrDefault(move => move.Id == moveId);
        if (move is null)
        {
            return NotFound();
        }
        else
        {
            return Json(MoveViewModel.FromModel(move));
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

        if (fromSea is null || toSea is null || !await _seaRepository.AreAccessible(fromSea, toSea))
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
