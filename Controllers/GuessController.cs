using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTFWhodunnit.Database;
using CTFWhodunnit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CTFWhodunnit.Controllers;

[Authorize]
public class GuessController : AuthenticatedController
{
    public readonly int MAXIMUM_GUESSES = 5;

    private readonly AppDbContext _context;
    private readonly ILogger<GuessController> _logger;

    public GuessController(AppDbContext context, ILogger<GuessController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var guesses = await _context.Guesses.Where(g => g.UserId == int.Parse(userId)).Include(g => g.Mastermind).ToListAsync();

        List<Suspect> guessedMasterminds = guesses.Select(g => g.Mastermind).ToList();
        var remainingSuspects = await _context.Suspects.Where(s => !guessedMasterminds.Contains(s)).ToListAsync();
        ViewBag.Options = remainingSuspects
            .Select(s => new SelectListItem
            {
                Value = s.SuspectId.ToString(),
                Text = s.Name
            })
            .ToList();

        var flags = _context.Flags.OrderByDescending(f => f.Points).Select(f => new FlagViewModel
        {
            Name = f.Name,
            Points = f.Points,
            Burnt = false,
            Value = f.Value
        }).ToList();

        for (int i = 0; i < guesses.Where(g => !g.Correct).Count() && i < flags.Count; i++)
        {
            flags[i].Burnt = true;
            flags[i].Value = null;
        }


        if (!guesses.Any(g => g.Correct))
        {
            flags.ForEach(f => f.Value = null);
        }

        ViewBag.Flags = flags;

        ViewData["RemainingGuesses"] = flags.Count() - guesses.Count();
        return View(guesses);
    }
    [HttpPost]
    public IActionResult Submit(int mastermindId)
    {
        Suspect guessedMastermind = _context.Suspects.FirstOrDefault(s => s.SuspectId == mastermindId);
        if (guessedMastermind == null)
        {
            ViewData["Error"] = "Invalid guess attempt!";
            return View("Error");
        }

        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        int attempts = _context.Guesses.Count(g => g.UserId == userId);
        if (attempts >= _context.Flags.Count())
        {
            ViewData["Error"] = "Maximum number of guesses reached!";
            return View("Error");
        }

        // Check if we've already made this guess or if we've already gussed correctly. In that case we don't save the current guess to the database
        var doNotStoreGuess = _context.Guesses.Any(g => g.UserId == userId && (g.Mastermind == guessedMastermind || g.Correct));
        if (!doNotStoreGuess)
        {
            var userGuess = new Guess
            {
                UserId = userId,
                Mastermind = guessedMastermind,
                TimeGuessed = DateTime.Now,
                Correct = guessedMastermind.IsCulprit
            };
            _context.Guesses.Add(userGuess);
            _context.SaveChanges();
        }

        if (guessedMastermind.IsCulprit)
        {
            return View("Success", guessedMastermind);
        }

        ViewData["Error"] = "Sorry, wrong guess!";
        return View("Error");
    }


}