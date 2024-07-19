using CTFWhodunnit.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PirateConquest.ViewModels;

namespace PirateConquest.Controllers;

public class TeamsController : Controller
{
    private readonly AppDbContext _context;

    public TeamsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("/api/teams/{teamId}")]
    public async Task<IActionResult> GetTeams(int? teamId)
    {
        if (teamId is int id)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(team => team.Id == id);
            if (team is null)
            {
                return NotFound();
            }
            else
            {
                return Json(TeamViewModel.FromModel(team));
            }
        }
        else
        {
            var allTeams = await _context.Teams.ToListAsync();
            return Json(allTeams.Select(TeamViewModel.FromModel));
        }
    }
}
