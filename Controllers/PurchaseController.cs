﻿using CTFWhodunnit.Database;
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

    public PurchaseController(AppDbContext context, SeaRepository seaRepository)
    {
        _context = context;
        _seaRepository = seaRepository;
    }

    [HttpGet("/api/purchases/{purchaseId}")]
    public async Task<IActionResult> GetTeamsPurchases(int? purchaseId)
    {
        var team = await User.GetTeamAsync(_context);
        if (team is null)
        {
            return Unauthorized();
        }

        if (purchaseId is int id)
        {
            var purchase = await _context.Purchases.FirstOrDefaultAsync(purchase =>
                purchase.Team == team && purchase.Id == id
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
        else
        {
            var allTeamPurchases = await _context
                .Purchases.Where(purchase => purchase.Team == team)
                .ToListAsync();
            return Json(allTeamPurchases);
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
        // TO SELF: check whether team can access sea
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
