using CTFWhodunnit.Database;
using Microsoft.EntityFrameworkCore;
using PirateConquest.Models;

namespace PirateConquest.Repositories;

public class OutcomeRepository
{
    public readonly AppDbContext _context;

    public OutcomeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Outcome>> All() =>
        await _context
            .Outcomes.Include(outcome => outcome.Round)
            .Include(outcome => outcome.Sea)
            .Include(outcome => outcome.Team)
            .ThenInclude(team => team.StartingSea)
            .OrderBy(outcome => outcome.Id)
            .ToListAsync();

    public async Task<List<Outcome>> LatestOutcomes()
    {
        var latestRound = await _context
            .Rounds.OrderByDescending(round => round.StartMoving)
            .FirstOrDefaultAsync();
        return (await All()).Where(outcome => outcome.Round.Id == latestRound.Id).ToList();
    }
}
