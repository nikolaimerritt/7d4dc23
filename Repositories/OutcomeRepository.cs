using Microsoft.EntityFrameworkCore;
using PirateConquest.Database;
using PirateConquest.Models;

namespace PirateConquest.Repositories;

public class OutcomeRepository
{
    private readonly AppDbContext _context;
    private readonly SeaRepository _seaRepository;
    private readonly RoundRepository _roundRepository;

    public OutcomeRepository(
        AppDbContext context,
        RoundRepository roundRepository,
        SeaRepository seaRepository
    )
    {
        _context = context;
        _roundRepository = roundRepository;
        _seaRepository = seaRepository;
    }

    public async Task<List<Outcome>> AllAsync()
    {
        var outcomes = await _context
            .Outcomes.Include(outcome => outcome.Round)
            .Include(outcome => outcome.Sea)
            .Include(outcome => outcome.Team)
            .ThenInclude(team => team.StartingSea)
            .OrderBy(outcome => outcome.Id)
            .ToListAsync();
        foreach (var outcome in outcomes)
        {
            outcome.Sea.AdjacentSeas = await _seaRepository.AdjacentSeasAsync(outcome.Sea);
        }
        return outcomes;
    }

    public async Task<List<Outcome>> InPreviousRoundAsync()
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

    public async Task<List<Sea>> GetAccessibleSeasAsync(Team team)
    {
        // TO SELF: very inefficient!
        var accessibleSeas = new List<Sea>();
        foreach (var sea in await _seaRepository.AllAsync())
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
        if (_seaRepository.AreAccessible(sea, team.StartingSea))
        {
            return true;
        }
        var latestTeamOutcomes = (await InPreviousRoundAsync())
            .Where(outcome => outcome.Team.Id == team.Id && outcome.ShipsAfter > 0)
            .ToList();
        foreach (var outcome in latestTeamOutcomes)
        {
            if (sea.IsAccessible(outcome.Sea))
            {
                return true;
            }
        }
        return false;
    }
}
