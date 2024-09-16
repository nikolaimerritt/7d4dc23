using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PirateConquest.Database;
using PirateConquest.Models;
using PirateConquest.Repositories;
using PirateConquest.Utils;

namespace PirateConquest.Controllers;

public class HomeController : Controller
{
    private readonly TeamRepository _teamRepository;

    public HomeController(TeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public async Task<IActionResult> Index()
    {
        var team = await _teamRepository.ByIdAsync(User.GetTeamId());
        if (team is null)
        {
            return RedirectToAction("Index", "Login");
        }
        return View();
    }

    [Authorize]
    public IActionResult Leaderboard()
    {
        return View();
    }

    public IActionResult History()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
