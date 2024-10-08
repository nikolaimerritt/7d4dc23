﻿using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PirateConquest.Database;
using PirateConquest.Models;
using PirateConquest.Utils;

namespace PirateConquest.Controllers;

[Authorize(Policy = "IsAdminPolicy")]
public class AdminController : Controller
{
    public AdminController() { }

    public async Task<IActionResult> Index()
    {
        return View();
    }

    //public async Task<IActionResult> ResetFlagValues()
    //{
    //    var maxFlagPointsConf = await _context.AppConfigs.FirstOrDefaultAsync(c =>
    //        c.Name == AppConfig.MAX_FLAG_POINTS_KEY
    //    );
    //    if (maxFlagPointsConf == null)
    //    {
    //        return View(
    //            new ErrorViewModel
    //            {
    //                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
    //            }
    //        );
    //    }
    //    Initializers.SetFlagValues(_context, int.Parse(maxFlagPointsConf.Value));
    //    return View("Success");
    //}

    //public async Task<IActionResult> ResetHintPoints()
    //{
    //    var maxCtfPointsConf = await _context.AppConfigs.FirstOrDefaultAsync(c =>
    //        c.Name == AppConfig.MAX_CTF_POINTS_KEY
    //    );
    //    if (maxCtfPointsConf == null)
    //    {
    //        return View(
    //            new ErrorViewModel
    //            {
    //                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
    //            }
    //        );
    //    }
    //    Initializers.SetHintUnlockPoints(_context, int.Parse(maxCtfPointsConf.Value));
    //    return View("Success");
    //}

    //public IActionResult UnlockIntel(bool forAll)
    //{
    //    _context.UnlockedIntels.RemoveRange(_context.UnlockedIntels);
    //    _context.SaveChanges();
    //    if (forAll)
    //    {
    //        Initializers.UnlockSuspectsForAll(_context);
    //    }
    //    else
    //    {
    //        Initializers.UnlockSuspectsByTeam(_context);
    //    }
    //    return View("Success");
    //}

    //public IActionResult LockIntelForAll()
    //{
    //    _context.UnlockedIntels.RemoveRange(_context.UnlockedIntels);
    //    _context.SaveChanges();
    //    return View("Success");
    //}

    //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    //public IActionResult Error()
    //{
    //    return View(
    //        new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
    //    );
    //}
}
