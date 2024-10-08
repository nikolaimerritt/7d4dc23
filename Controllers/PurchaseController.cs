﻿using Microsoft.AspNetCore.Mvc;
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
    private readonly OutcomeRepository _outcomeRepository;
    private readonly ConfigurationRepository _configurationRepository;

    public PurchaseController(
        SeaRepository seaRepository,
        PurchaseRepository purchaseRepository,
        TeamRepository teamRepository,
        RoundRepository roundRepository,
        PointsService pointsService,
        OutcomeRepository outcomeRepository,
        ConfigurationRepository configurationRepository
    )
    {
        _seaRepository = seaRepository;
        _purchaseRepository = purchaseRepository;
        _teamRepository = teamRepository;
        _roundRepository = roundRepository;
        _pointsService = pointsService;
        _outcomeRepository = outcomeRepository;
        _configurationRepository = configurationRepository;
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

        var configuration = await _configurationRepository.GetNonEmptyAsync();
        var availablePoints = await AvailablePoints(team);
        if (availablePoints is null || configuration is null)
        {
            return Json(ErrorViewModel.InternalError);
        }
        if (points < 0 || points > availablePoints)
        {
            return Json(ErrorViewModel.NotEnoughPoints);
        }

        var sea = await _seaRepository.ByIdAsync(seaId);
        if (sea is null || !await _outcomeRepository.TeamCanAccess(team, sea))
        {
            return Json(ErrorViewModel.SeasAreInaccessible);
        }

        var shipsBought = (int)Math.Floor(points / (double)configuration.PointsPerShip);
        var pointsSpent = shipsBought * configuration.PointsPerShip;
        await _purchaseRepository.AddAsync(
            new Purchase()
            {
                Team = team,
                Round = round,
                Sea = sea,
                ShipCount = shipsBought,
                Points = pointsSpent,
                Creation = DateTime.UtcNow
            }
        );
        return Json(new OkViewModel());
    }

    private async Task<int?> AvailablePoints(Team team)
    {
        var config = await _configurationRepository.GetNonEmptyAsync();
        var pointsGained = 99;
        if (config is null || !config.DebugAssignMaxPoints)
        {
            pointsGained = await _pointsService.GetPointsAsync(team);
        }
        var pointsSpent = (await _purchaseRepository.AllAsync())
            .Where(purchase => purchase.Team == team)
            .Sum(purchase => purchase.Points);
        return Math.Max(pointsGained - pointsSpent, 0);
    }
}
