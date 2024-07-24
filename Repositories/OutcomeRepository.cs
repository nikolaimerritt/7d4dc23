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

    public async Task<List<Outcome>> FromPreviousRoundAsync()
    {
        var latestRound = await _roundRepository.GetPreviousRoundAsync();
        return await FromRoundAsync(latestRound);
    }

    public async Task<List<Outcome>> FromRoundAsync(Round round) =>
        (await All()).Where(outcome => outcome.Round.Id == round.Id).ToList();

    public async Task AddIfNotExists(Outcome toAdd)
    {
        if (
            !await _context.Outcomes.AnyAsync(outcome =>
                outcome.SeaId == toAdd.SeaId
                && outcome.RoundId == toAdd.RoundId
                && outcome.TeamId == toAdd.TeamId
            )
        )
        {
            await _context.Outcomes.AddAsync(toAdd);
            await _context.SaveChangesAsync();
        }
    }
}
