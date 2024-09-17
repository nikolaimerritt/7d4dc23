using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PirateConquest.Database;
using PirateConquest.Models;
using PirateConquest.Repositories;
using PirateConquest.Utils;
using PirateConquest.ViewModels;

namespace PirateConquest.Controllers;

public class MoveController : Controller
{
    private readonly SeaRepository _seaRepository;
    private readonly MoveRepository _moveRepository;
    private readonly TeamRepository _teamRepository;
    private readonly RoundRepository _roundRepository;

    public MoveController(
        SeaRepository seaRepository,
        MoveRepository moveRepository,
        TeamRepository teamRepository,
        RoundRepository roundRepository
    )
    {
        _seaRepository = seaRepository;
        _moveRepository = moveRepository;
        _teamRepository = teamRepository;
        _roundRepository = roundRepository;
    }

    [HttpGet("/api/moves")]
    public async Task<IActionResult> GetMoves(int? roundId = null)
    {
        var moves = await _moveRepository.AllAsync();
        if (roundId is int id)
        {
            moves = moves.Where(move => move.RoundId == id).ToList();
        }
        return Json(moves.Select(MoveViewModel.FromModel));
    }

    [HttpGet("/api/moves/{moveId}")]
    public async Task<IActionResult> GetTeamMove(int moveId)
    {
        var team = await _teamRepository.ByIdAsync(User.GetTeamId());
        if (team is null)
        {
            return Json(ErrorViewModel.Unauthorized);
        }

        var move = (await _moveRepository.AllAsync()).FirstOrDefault(move => move.Id == moveId);
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
            return Json(ErrorViewModel.Unauthorized);
        }
        var round = await _roundRepository.GetCurrentRoundAsync();
        if (round?.StartCooldown < DateTime.UtcNow)
        {
            return Json(ErrorViewModel.PlanningWindowHasEnded);
        }

        var fromSea = await _seaRepository.ByIdAsync(fromSeaId);
        var toSea = await _seaRepository.ByIdAsync(toSeaId);
        if (fromSea is null || toSea is null || !fromSea.IsAccessible(toSea))
        {
            return Json(ErrorViewModel.SeasAreInaccessible);
        }

        var availableShips = await _roundRepository.CountTeamShipsAsync(fromSea, team);
        if (shipCount < 0 || shipCount > availableShips)
        {
            return Json(ErrorViewModel.NotEnoughShips);
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
        return Json(new OkViewModel());
    }
}
