using CTFWhodunnit.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PirateConquest.Models;
using PirateConquest.Repositories;
using PirateConquest.Utils;
using PirateConquest.ViewModels;

namespace PirateConquest.Controllers;

public class PurchaseController : Controller
{
    private readonly AppDbContext _context;
    private readonly SeaRepository _seaRepository;
    private readonly PurchaseRepository _purchaseRepository;
    private readonly TeamRepository _teamRepository;
    private readonly RoundRepository _roundRepository;

    public PurchaseController(
        AppDbContext context,
        SeaRepository seaRepository,
        PurchaseRepository purchaseRepository,
        TeamRepository teamRepository,
        RoundRepository roundRepository
    )
    {
        _context = context;
        _seaRepository = seaRepository;
        _purchaseRepository = purchaseRepository;
        _teamRepository = teamRepository;
        _roundRepository = roundRepository;
    }

    [HttpGet("/api/purchases")]
    public async Task<IActionResult> GetPurchases(int? roundId = null)
    {
        var purchases = await _purchaseRepository.All();
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

        var purchase = (await _purchaseRepository.All()).FirstOrDefault(purchase =>
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
        if (round?.StartFighting < DateTime.UtcNow)
        {
            return Json(ErrorViewModel.PlanningWindowHasEnded);
        }

        var currentRound = await _roundRepository.GetCurrentRoundAsync();
        if (currentRound is null)
        {
            return BadRequest();
        }
        if (points < 0 || points > await AvailablePoints(team))
        {
            return Json(ErrorViewModel.NotEnoughPoints);
        }
        var sea = await _context.Seas.FirstOrDefaultAsync(sea => sea.Id == seaId);
        if (sea is null || !await _seaRepository.TeamCanAccess(team, sea))
        {
            return Json(ErrorViewModel.SeasAreInaccessible);
        }

        await _context.Purchases.AddAsync(
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
        await _context.SaveChangesAsync();
        return Json(new OkViewModel());
    }

    private async Task<int> AvailablePoints(Team team)
    {
        var pointsGained = 99; // TO SELF: query Playground for teams points
        var pointsSpent = await _context
            .Purchases.Where(purchase => purchase.Team == team)
            .SumAsync(purchase => purchase.Points);
        return pointsGained - pointsSpent;
    }
}
