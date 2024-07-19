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

    public async Task<List<Outcome>> LatestOutcomes()
    {
        var latestRound = await _context
            .Rounds.OrderByDescending(round => round.StartMoving)
            .FirstOrDefaultAsync();
        return await _context.Outcomes.Where(outcome => outcome.Round == latestRound).ToListAsync();
    }
}
