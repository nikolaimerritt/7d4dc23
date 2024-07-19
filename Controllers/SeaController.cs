using CTFWhodunnit.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PirateConquest.Models;
using PirateConquest.Repositories;
using PirateConquest.ViewModels;

namespace PirateConquest.Controllers;

public class SeaController : Controller
{
    private readonly AppDbContext _context;
    private readonly SeaRepository _seaRepository;

    public SeaController(AppDbContext context, SeaRepository seaRepository)
    {
        _context = context;
        _seaRepository = seaRepository;
    }

    [HttpGet("/api/seas/{seaId}")]
    public async Task<IActionResult> GetSeas(int? seaId)
    {
        if (seaId is int id)
        {
            var sea = await _context.Seas.FirstOrDefaultAsync(sea => sea.Id == id);
            if (sea is null)
            {
                return NotFound();
            }
            else
            {
                return Json(await GetSeaViewModel(sea));
            }
        }
        else
        {
            var allSeas = await _context.Seas.ToListAsync();
            return Json(await Task.WhenAll(allSeas.Select(GetSeaViewModel)));
        }
    }

    private async Task<SeaViewModel> GetSeaViewModel(Sea sea)
    {
        var adjacentSeas = await _seaRepository.AdjacentSeas(sea);
        return SeaViewModel.FromModel(sea, adjacentSeas);
    }
}
