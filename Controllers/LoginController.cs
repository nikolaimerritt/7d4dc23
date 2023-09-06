using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CTFWhodunnit.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> Authenticate(string username, string password)
    {
        // Remember to hash and salt passwords in a real scenario
        var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

        if (user != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim("IsAdmin", user.IsAdmin.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync("CookieAuth", claimsPrincipal);

            return RedirectToAction("Index", "Home");
        }

        ViewData["Error"] = "Invalid username or password.";
        return View("Index");
    }
}
