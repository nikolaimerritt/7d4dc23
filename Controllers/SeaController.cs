﻿using Microsoft.AspNetCore.Mvc;
using PirateConquest.Repositories;
using PirateConquest.Utils;
using PirateConquest.ViewModels;

namespace PirateConquest.Controllers;

public class SeaController : Controller
{
    private readonly SeaRepository _seaRepository;
    private readonly TeamRepository _teamRepository;
    private readonly OutcomeRepository _outcomeRepository;

    public SeaController(
        SeaRepository seaRepository,
        TeamRepository teamRepository,
        OutcomeRepository outcomeRepository
    )
    {
        _seaRepository = seaRepository;
        _teamRepository = teamRepository;
        _outcomeRepository = outcomeRepository;
    }

    [HttpGet("/api/seas")]
    public async Task<IActionResult> GetAllSeas()
    {
        var allSeas = await _seaRepository.AllAsync();
        return Json(allSeas.Select(SeaViewModel.FromModel));
    }

    [HttpGet("/api/seas/accessible")]
    public async Task<IActionResult> GetAccessibleSeas()
    {
        var team = await _teamRepository.ByIdAsync(User.GetTeamId());
        if (team is null)
        {
            return Json(ErrorViewModel.Unauthorized);
        }
        var accessibleSeas = await _outcomeRepository.GetAccessibleSeasAsync(team);
        return Json(accessibleSeas.Select(SeaViewModel.FromModel));
    }

    [HttpGet("/api/seas/{seaId}")]
    public async Task<IActionResult> GetSea(int seaId)
    {
        var sea = (await _seaRepository.AllAsync()).Find(sea => sea.Id == seaId);
        if (sea is null)
        {
            return NotFound();
        }
        else
        {
            return Json(SeaViewModel.FromModel(sea));
        }
    }
}
