using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PirateConquest.Database;
using PirateConquest.Models;
using PirateConquest.Repositories;
using PirateConquest.Utils;
using PirateConquest.ViewModels;

namespace PirateConquest.Controllers;

public class SeaController : Controller
{
    private readonly AppDbContext _context;
    private readonly SeaRepository _seaRepository;
    private readonly TeamRepository _teamRepository;

    public SeaController(
        AppDbContext context,
        SeaRepository seaRepository,
        TeamRepository teamRepository
    )
    {
        _context = context;
        _seaRepository = seaRepository;
        _teamRepository = teamRepository;
    }

    [HttpGet("/api/seas")]
    public async Task<IActionResult> GetAllSeas()
    {
        var allSeas = await _context.Seas.ToListAsync();
        return Json(await Task.WhenAll(allSeas.Select(GetSeaViewModel)));
    }

    [HttpGet("/api/seas/accessible")]
    public async Task<IActionResult> GetAccessibleSeas()
    {
        var team = await _teamRepository.ByIdAsync(User.GetTeamId());
        if (team is null)
        {
            return Json(ErrorViewModel.Unauthorized);
        }
        var accessibleSeas = await _seaRepository.GetAccessibleSeasAsync(team);
        return Json(await Task.WhenAll(accessibleSeas.Select(GetSeaViewModel)));
    }

    [HttpGet("/api/seas/{seaId}")]
    public async Task<IActionResult> GetSea(int seaId)
    {
        var sea = await _context.Seas.FirstOrDefaultAsync(sea => sea.Id == seaId);
        if (sea is null)
        {
            return NotFound();
        }
        else
        {
            return Json(await GetSeaViewModel(sea));
        }
    }

    private async Task<SeaViewModel> GetSeaViewModel(Sea sea)
    {
        var adjacentSeas = await _seaRepository.AdjacentSeas(sea);
        return SeaViewModel.FromModel(sea, adjacentSeas);
    }
}
