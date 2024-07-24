using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CTFWhodunnit.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CTFWhodunnit.Controllers;

public class LoginController : Controller
{
    private readonly AppDbContext _context;

    public LoginController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CookieAuth"); // The string argument should match the scheme you've configured

        // Redirect to home page or login page
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Authenticate(string teamName, string password)
    {
        var team = await _context.Teams.FirstOrDefaultAsync(team =>
            team.Name == teamName && team.PlainTextPassword == password
        );

        if (team is null)
        {
            ViewData["ErrorViewModel"] = "Invalid TeamViewModel Name or Password.";
            return View("Index");
        }
        else
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, team.Name),
                new(ClaimTypes.NameIdentifier, team.Id.ToString()),
                new(ClaimTypes.Sid, team.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync("CookieAuth", claimsPrincipal);
            return RedirectToAction("Index", "Home");
        }
    }
}
