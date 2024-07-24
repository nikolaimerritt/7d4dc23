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
    private readonly TeamRepository _teamRepository;
    private readonly RoundRepository _roundRepository;

    public MoveController(
        AppDbContext context,
        SeaRepository seaRepository,
        MoveRepository moveRepository,
        TeamRepository teamRepository,
        RoundRepository roundRepository
    )
    {
        _context = context;
        _seaRepository = seaRepository;
        _moveRepository = moveRepository;
        _teamRepository = teamRepository;
        _roundRepository = roundRepository;
    }

    [HttpGet("/api/moves")]
    public async Task<IActionResult> GetTeamMoves()
    {
        var team = await _teamRepository.ByIdAsync(User.GetTeamId());
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
        var team = await _teamRepository.ByIdAsync(User.GetTeamId());
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
        var team = await _teamRepository.ByIdAsync(User.GetTeamId());
        if (team is null)
        {
            return Unauthorized();
        }
        var round = await _roundRepository.GetCurrentRoundAsync();
        if (round?.StartFighting < DateTime.UtcNow)
        {
            return BadRequest();
        }

        var fromSea = await _context.Seas.FirstOrDefaultAsync(sea => sea.Id == fromSeaId);
        var toSea = await _context.Seas.FirstOrDefaultAsync(sea => sea.Id == toSeaId);

        if (fromSea is null || toSea is null || !await _seaRepository.AreAccessible(fromSea, toSea))
        {
            return BadRequest();
        }

        var availableShips = await _roundRepository.CountTeamShipsAsync(fromSea, team);
        if (shipCount < 0 || shipCount > availableShips)
        {
            return BadRequest();
        }

        await _moveRepository.AddIfNotExistsAsync(
            new()
            {
                Round = await _roundRepository.GetCurrentRoundAsync(),
                Team = team,
                FromSea = fromSea,
                ToSea = toSea,
                ShipCount = shipCount,
                Creation = DateTime.UtcNow
            }
        );
        return Json(null);
    }
}
