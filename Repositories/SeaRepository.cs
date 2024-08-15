using Microsoft.EntityFrameworkCore;
using PirateConquest.Database;
using PirateConquest.Models;
using SQLitePCL;

namespace PirateConquest.Repositories;

public class SeaRepository
{
    private readonly AppDbContext _context;

    public SeaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Sea>> AllAsync()
    {
        var seas = await _context.Seas.OrderBy(sea => sea.Name).ToListAsync();
        foreach (var sea in seas)
        {
            sea.AdjacentSeas = await AdjacentSeasAsync(sea);
        }
        return seas;
    }

    public async Task<Sea?> ByIdAsync(int seaId)
    {
        var sea = await _context.Seas.FirstOrDefaultAsync(sea => sea.Id == seaId);
        if (sea is not null)
        {
            sea.AdjacentSeas = await AdjacentSeasAsync(sea);
        }
        return sea;
    }

    public async Task<List<Sea>> AdjacentSeasAsync(Sea sea)
    {
        var seas = await _context
            .AdjacentSeas.Where(adjacentSea => adjacentSea.Sea == sea)
            .Select(adjacentSea => adjacentSea.AdjacentTo)
            .ToListAsync();
        return seas.Select(sea => new Sea()
            {
                Id = sea.Id,
                Name = sea.Name,
                AdjacentSeas = new()
            })
            .ToList();
    }

    public bool AreAccessible(Sea first, Sea second) =>
        first.Id == second.Id
        || first.AdjacentSeas.Any(sea => sea.Id == second.Id)
        || second.AdjacentSeas.Any(sea => sea.Id == first.Id);
}
