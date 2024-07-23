using CTFWhodunnit.Database;
using Microsoft.EntityFrameworkCore;
using PirateConquest.Models;

namespace PirateConquest.Repositories;

public class OutcomeRepository
{
    private readonly AppDbContext _context;
    private readonly RoundRepository _roundRepository;

    public OutcomeRepository(AppDbContext context, RoundRepository roundRepository)
    {
        _context = context;
        _roundRepository = roundRepository;
    }

    public async Task<List<Outcome>> All() =>
        await _context
            .Outcomes.Include(outcome => outcome.Round)
            .Include(outcome => outcome.Sea)
            .Include(outcome => outcome.Team)
            .ThenInclude(team => team.StartingSea)
            .OrderBy(outcome => outcome.Id)
            .ToListAsync();

    public async Task<List<Outcome>> FromLatestRound()
    {
        var latestRound = await _roundRepository.GetLatestRoundAsync();
        return await FromRoundAsync(latestRound);
    }

    public async Task<List<Outcome>> FromRoundAsync(Round round) =>
        (await All()).Where(outcome => outcome.Round.Id == round.Id).ToList();

    public async Task Add(IEnumerable<Outcome> outcomes)
    {
        foreach (var outcome in outcomes)
        {
            outcome.RoundId = outcome.Round.Id;
            outcome.Round = null;
        }
        await _context.Outcomes.AddRangeAsync(outcomes);
        await _context.SaveChangesAsync();
    }
}
