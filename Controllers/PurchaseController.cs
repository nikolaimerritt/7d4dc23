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

    public PurchaseController(
        AppDbContext context,
        SeaRepository seaRepository,
        PurchaseRepository purchaseRepository
    )
    {
        _context = context;
        _seaRepository = seaRepository;
        _purchaseRepository = purchaseRepository;
    }

    [HttpGet("/api/purchases")]
    public async Task<IActionResult> GetAllTeamsPurchases()
    {
        var team = await User.GetTeamAsync(_context);
        if (team is null)
        {
            return Unauthorized();
        }
        var allTeamPurchases = (await _purchaseRepository.All()).Where(purchase =>
            purchase.Team == team
        );
        return Json(allTeamPurchases);
    }

    [HttpGet("/api/purchases/{purchaseId}")]
    public async Task<IActionResult> GetTeamsPurchases(int purchaseId)
    {
        var team = await User.GetTeamAsync(_context);
        if (team is null)
        {
            return Unauthorized();
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
            return Json(purchase);
        }
    }

    [HttpPut("/api/purchases")]
    public async Task<IActionResult> PutPurchase(int points, int seaId)
    {
        var team = await User.GetTeamAsync(_context);
        if (team is null)
        {
            return Unauthorized();
        }

        var movingRound = await _context.GetMovingRoundAsync();
        var sea = await _context.Seas.FirstOrDefaultAsync(sea => sea.Id == seaId);

        if (sea is null || !await _seaRepository.TeamCanAccess(team, sea))
        {
            return BadRequest();
        }

        if (movingRound is null)
        {
            return BadRequest();
        }
        if (points < 0 || points > await AvailablePoints(team))
        {
            return BadRequest();
        }

        await _context.Purchases.AddAsync(
            new Purchase()
            {
                Team = team,
                Round = movingRound,
                Points = points,
                ShipCount = points,
                Creation = DateTime.UtcNow
            }
        );
        await _context.SaveChangesAsync();
        return Ok();
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
