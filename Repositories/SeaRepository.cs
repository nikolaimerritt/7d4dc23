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

    public async Task<List<Sea>> AllAsync() => await _context.Seas.ToListAsync();

    public async Task<List<Sea>> AdjacentSeas(Sea sea) =>
        await _context
            .AdjacentSeas.Where(adjacentSea => adjacentSea.Sea == sea)
            .Select(adjacentSea => adjacentSea.AdjacentTo)
            .ToListAsync();

    public async Task<bool> AreAccessible(Sea first, Sea second) =>
        first.Id == second.Id
        || (await AdjacentSeas(first)).Any(sea => sea.Id == second.Id)
        || (await AdjacentSeas(second)).Any(sea => sea.Id == first.Id);

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
            if (await AreAccessible(outcome.Sea, sea))
            {
                return true;
            }
        }
        return false;
    }
}
