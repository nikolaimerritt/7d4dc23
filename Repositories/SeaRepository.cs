using Microsoft.EntityFrameworkCore;
using PirateConquest.Database;
using PirateConquest.Models;
using SQLitePCL;

namespace PirateConquest.Repositories;

public class SeaRepository
{
    private readonly AppDbContext _context;
    private readonly OutcomeRepository _outcomeRepository;

    public SeaRepository(AppDbContext context, OutcomeRepository outcomeRepository)
    {
        _context = context;
        _outcomeRepository = outcomeRepository;
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

    public async Task<List<Sea>> AdjacentSeasAsync(Sea sea) =>
        await _context
            .AdjacentSeas.Where(adjacentSea => adjacentSea.Sea == sea)
            .Select(adjacentSea => adjacentSea.AdjacentTo)
            .ToListAsync();

    public bool AreAccessible(Sea first, Sea second) =>
        first.Id == second.Id
        || first.AdjacentSeas.Any(sea => sea.Id == second.Id)
        || second.AdjacentSeas.Any(sea => sea.Id == first.Id);

    public async Task<List<Sea>> GetAccessibleSeasAsync(Team team)
    {
        // TO SELF: very inefficient!
        var accessibleSeas = new List<Sea>();
        foreach (var sea in await AllAsync())
        {
            if (await TeamCanAccess(team, sea))
            {
                accessibleSeas.Add(sea);
            }
        }
        return accessibleSeas;
    }

    public async Task<bool> TeamCanAccess(Team team, Sea sea)
    {
        var latestTeamOutcomes = (await _outcomeRepository.InPreviousRoundAsync())
            .Where(outcome => outcome.Team.Id == team.Id)
            .ToList();
        foreach (var outcome in latestTeamOutcomes)
        {
            if (AreAccessible(outcome.Sea, sea))
            {
                return true;
            }
        }
        return false;
    }
}
