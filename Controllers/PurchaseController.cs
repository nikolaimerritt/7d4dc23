using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PirateConquest.Database;
using PirateConquest.Models;
using PirateConquest.Repositories;
using PirateConquest.Services;
using PirateConquest.Utils;
using PirateConquest.ViewModels;

namespace PirateConquest.Controllers;

public class PurchaseController : Controller
{
    private readonly SeaRepository _seaRepository;
    private readonly PurchaseRepository _purchaseRepository;
    private readonly TeamRepository _teamRepository;
    private readonly RoundRepository _roundRepository;
    private readonly PointsService _pointsService;

    public PurchaseController(
        SeaRepository seaRepository,
        PurchaseRepository purchaseRepository,
        TeamRepository teamRepository,
        RoundRepository roundRepository,
        PointsService pointsService
    )
    {
        _seaRepository = seaRepository;
        _purchaseRepository = purchaseRepository;
        _teamRepository = teamRepository;
        _roundRepository = roundRepository;
        _pointsService = pointsService;
    }

    [HttpGet("/api/purchases")]
    public async Task<IActionResult> GetPurchases(int? roundId = null)
    {
        var purchases = await _purchaseRepository.AllAsync();
        if (roundId is int id)
        {
            purchases = purchases.Where(purchase => purchase.Round.Id == id).ToList();
        }
        return Json(purchases.Select(PurchaseViewModel.FromModel));
    }

    [HttpGet("/api/purchases/balance")]
    public async Task<IActionResult> GetBalance()
    {
        var team = await _teamRepository.ByIdAsync(User.GetTeamId());
        if (team is null)
        {
            return Json(ErrorViewModel.Unauthorized);
        }
        else
        {
            return Json(await AvailablePoints(team));
        }
    }

    [HttpGet("/api/purchases/{purchaseId}")]
    public async Task<IActionResult> GetTeamsPurchases(int purchaseId)
    {
        var team = await _teamRepository.ByIdAsync(User.GetTeamId());
        if (team is null)
        {
            return Json(ErrorViewModel.Unauthorized);
        }

        var purchase = (await _purchaseRepository.AllAsync()).FirstOrDefault(purchase =>
            purchase.Team == team && purchase.Id == purchaseId
        );
        if (purchase is null)
        {
            return NotFound();
        }
        else
        {
            return Json(PurchaseViewModel.FromModel(purchase));
        }
    }

    [HttpPut("/api/purchases")]
    public async Task<IActionResult> PutPurchase(int points, int seaId)
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

        var currentRound = await _roundRepository.GetCurrentRoundAsync();
        if (currentRound is null)
        {
            return BadRequest();
        }
        var availablePoints = await AvailablePoints(team);
        if (availablePoints is null)
        {
            return Json(ErrorViewModel.InternalError);
        }
        if (points < 0 || points > availablePoints)
        {
            return Json(ErrorViewModel.NotEnoughPoints);
        }

        var sea = await _seaRepository.ByIdAsync(seaId);
        if (sea is null || !await _seaRepository.TeamCanAccess(team, sea))
        {
            return Json(ErrorViewModel.SeasAreInaccessible);
        }

        await _purchaseRepository.AddAsync(
            new Purchase()
            {
                Team = team,
                Round = currentRound,
                Sea = sea,
                Points = points,
                ShipCount = points,
                Creation = DateTime.UtcNow
            }
        );
        return Json(new OkViewModel());
    }

    private async Task<int?> AvailablePoints(Team team)
    {
        //var pointsGained = await _pointsService.GetPointsAsync(team);
        var pointsGained = 99;
        var pointsSpent = (await _purchaseRepository.AllAsync())
            .Where(purchase => purchase.Team == team)
            .Sum(purchase => purchase.Points);
        return pointsGained - pointsSpent;
    }
}
