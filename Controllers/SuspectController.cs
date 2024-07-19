using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CTFWhodunnit.Database;
using CTFWhodunnit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace CTFWhodunnit.Controllers;

[Authorize]
public class SuspectController : AuthenticatedController
{
    private readonly AppDbContext _context;
    private static string _secretKey = "734d9104-df77-4b25-853e-e295d743a1ea";
    private static readonly string OBFUSCATION_STRING = "********";
    private readonly ILogger<SuspectController> _logger;

    public SuspectController(AppDbContext context, ILogger<SuspectController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IActionResult List()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var suspects = _context
            .Suspects.ToList()
            .Select(suspect =>
            {
                var isUnlocked = _context.UnlockedIntels.Any(ui =>
                    ui.UserId == userId && ui.Suspect.SuspectId == suspect.SuspectId
                );
                return new SuspectViewModel
                {
                    SuspectId = suspect.SuspectId,
                    Name = suspect.Name,
                    Location = isUnlocked ? suspect.Location : OBFUSCATION_STRING,
                    OperatingSystem = isUnlocked ? suspect.OperatingSystem : OBFUSCATION_STRING,
                    Skills = isUnlocked ? suspect.Skills : OBFUSCATION_STRING,
                    Secret = isUnlocked ? ComputeHmac(suspect.Name) : null
                };
            })
            .OrderByDescending(suspect => suspect.Secret != null)
            .ToList();
        return Json(suspects);
    }

    [HttpGet]
    public IActionResult UnlockIntel(int suspectId, string secret)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var suspect = _context.Suspects.FirstOrDefault(s => s.SuspectId == suspectId);
        if (suspect == null)
        {
            ViewData["Error"] = "Suspect not found!";
            return View("Error");
        }

        var computedSecret = ComputeHmac(suspect.Name);
        if (computedSecret != secret)
        {
            ViewData["Error"] = "You received an invalid piece of intel!";
            return View("Error");
        }

        var alreadyUnlocked = _context.UnlockedIntels.Any(ui =>
            ui.UserId == userId && ui.Suspect == suspect
        );
        if (alreadyUnlocked)
        {
            ViewData["Message"] = "You already had this piece of intel!";
            return View();
        }

        var unlockedIntel = new UnlockedIntel
        {
            UserId = userId,
            Suspect = suspect,
            TimeUnlocked = DateTime.Now
        };

        _context.UnlockedIntels.Add(unlockedIntel);
        _context.SaveChanges();
        ViewData["Message"] = "Well done, you have unlocked new intel about a suspect!";
        return View();
    }

    private static String ComputeHmac(string input)
    {
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey)))
        {
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }

    public IActionResult Index()
    {
        return View();
    }
}
