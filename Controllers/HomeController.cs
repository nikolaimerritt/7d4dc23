using System.Diagnostics;
using CTFWhodunnit.Database;
using CTFWhodunnit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PirateConquest.Repositories;
using PirateConquest.Utils;

namespace CTFWhodunnit.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;
    private readonly TeamRepository _teamRepository;

    public HomeController(
        ILogger<HomeController> logger,
        AppDbContext context,
        TeamRepository teamRepository
    )
    {
        _logger = logger;
        _context = context;
        _teamRepository = teamRepository;
    }

    public async Task<IActionResult> Index()
    {
        var team = await _teamRepository.ByIdAsync(User.GetTeamId());
        if (team is null)
        {
            return RedirectToAction("Index", "Login");
        }
        var videoUrlConf = await _context.AppConfigs.FirstOrDefaultAsync(c =>
            c.Name == AppConfig.VIDEO_URL_KEY
        );
        ViewBag.VideoUrl = videoUrlConf.Value;
        return View();
    }

    [Authorize]
    public IActionResult Schedule()
    {
        return View();
    }

    [Authorize]
    public IActionResult Leaderboard()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
