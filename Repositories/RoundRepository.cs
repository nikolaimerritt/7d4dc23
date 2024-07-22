using CTFWhodunnit.Database;
using Microsoft.EntityFrameworkCore;
using PirateConquest.Models;

namespace PirateConquest.Repositories;

public class RoundRepository
{
    private readonly AppDbContext _context;

    public RoundRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Round?> GetCurrentRound()
    {
        var now = DateTime.UtcNow;
        return await _context.Rounds.FirstOrDefaultAsync(round =>
            round.StartMoving <= now && now < round.StartFighting
        );
    }

    public async Task<List<Round>> AllPlayableRounds() =>
        await _context
            .Rounds.Where(round => !round.IsInitial)
            .OrderBy(round => round.End)
            .ToListAsync();

    public async Task<Round?> GetLastRound()
    {
        var now = DateTime.UtcNow;
        return await _context
            .Rounds.OrderByDescending(round => round.End)
            .FirstOrDefaultAsync(round => round.End < now);
    }

    public async Task<Round?> RoundBefore(Round round) =>
        await _context
            .Rounds.OrderByDescending(roundBefore => roundBefore.End)
            .FirstOrDefaultAsync(roundBefore => roundBefore.End < round.End);

    public async Task<int> TeamShipCount(Round round, Sea sea, Team team)
    {
        var previousRound = await RoundBefore(round);
        var prevoousOutcome = await _context
            .Outcomes.Include(outcome => outcome.Round)
            .Include(outcome => outcome.Team)
            .Include(outcome => outcome.Sea)
            .FirstOrDefaultAsync(outcome =>
                outcome.Round.Id == previousRound.Id
                && outcome.Team.Id == team.Id
                && outcome.Sea.Id == sea.Id
            );

        var movesToSea = await _context
            .Moves.Include(move => move.Round)
            .Include(move => move.Team)
            .Include(move => move.ToSea)
            .Where(move =>
                move.Round.Id == round.Id && move.ToSea.Id == sea.Id && move.Team.Id == team.Id
            )
            .ToListAsync();

        return prevoousOutcome?.ShipCount ?? 0 + movesToSea.Sum(move => move.ShipCount);
    }
}
