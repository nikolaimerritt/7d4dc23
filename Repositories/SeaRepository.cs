using CTFWhodunnit.Database;
using Microsoft.EntityFrameworkCore;
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

    public async Task<List<Sea>> AdjacentSeas(Sea sea) =>
        await _context
            .AdjacentSeas.Where(adjacentSea => adjacentSea.Sea == sea)
            .Select(adjacentSea => adjacentSea.AdjacentTo)
            .ToListAsync();

    public async Task<bool> AreAccessible(Sea sea, Sea other) =>
        sea.Id == other.Id || (await AdjacentSeas(sea)).Any(sea => sea.Id == other.Id);

    public async Task<bool> TeamCanAccess(Team team, Sea sea)
    {
        if (await AreAccessible(team.StartingSea, sea))
        {
            return true;
        }

        var latestTeamOutcomes = (await _outcomeRepository.LatestOutcomes()).Where(outcome =>
            outcome.Team.Id == team.Id
        );
        return latestTeamOutcomes.Any(outcome => outcome.Sea.Id == sea.Id);
    }
}
