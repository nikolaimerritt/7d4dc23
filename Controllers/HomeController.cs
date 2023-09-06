using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CTFWhodunnit.Models;
using Microsoft.AspNetCore.Authorization;
using CTFWhodunnit.Database;
using Microsoft.EntityFrameworkCore;

namespace CTFWhodunnit.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;


    public HomeController(ILogger<HomeController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var videoUrlConf = await _context.AppConfigs.FirstOrDefaultAsync(c => c.Name == AppConfig.VIDEO_URL_KEY);
        if (videoUrlConf == null)
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        }
        ViewBag.VideoUrl = videoUrlConf.Value;
        return View();
    }

    [Authorize]
    public IActionResult Schedule()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
