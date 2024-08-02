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

    public async Task<List<Outcome>> AllAsync() =>
        await _context
            .Outcomes.Include(outcome => outcome.Round)
            .Include(outcome => outcome.Sea)
            .Include(outcome => outcome.Team)
            .ThenInclude(team => team.StartingSea)
            .OrderBy(outcome => outcome.Id)
            .ToListAsync();

    public async Task<List<Outcome>> FromPreviousRoundAsync()
    {
        var previousRound = await _roundRepository.GetPreviousRoundAsync();
        return await WithinAsync(previousRound);
    }

    public async Task<List<Outcome>> WithinAsync(Round round) =>
        (await AllAsync()).Where(outcome => outcome.Round.Id == round.Id).ToList();

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

    public async Task<List<Outcome>> WithinAsync(Round round, Sea sea) =>
        (await AllAsync())
            .Where(outcome => outcome.RoundId == round.Id && outcome.SeaId == sea.Id)
            .ToList();
}
