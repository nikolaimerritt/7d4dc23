﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PirateConquest.Database;
using PirateConquest.Repositories;
using PirateConquest.Utils;
using PirateConquest.ViewModels;

namespace PirateConquest.Controllers;

public class TeamsController : Controller
{
    private readonly TeamRepository _teamRepository;

    public TeamsController(TeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    [HttpGet("/api/teams")]
    public async Task<IActionResult> GetAllTeams()
    {
        var allTeams = await _teamRepository.AllAsync();
        return Json(allTeams.Select(TeamViewModel.FromModel));
    }

    [HttpGet("/api/teams/self")]
    public async Task<IActionResult> GetOwnTeam()
    {
        var team = await _teamRepository.ByIdAsync(User.GetTeamId());
        if (team is null)
        {
            return NotFound();
        }
        else
        {
            return Json(TeamViewModel.FromModel(team));
        }
    }

    [HttpGet("/api/teams/{teamId}")]
    public async Task<IActionResult> GetTeam(int teamId)
    {
        var team = await _teamRepository.ByIdAsync(teamId);
        if (team is null)
        {
            return NotFound();
        }
        else
        {
            return Json(TeamViewModel.FromModel(team));
        }
    }
}
