using Microsoft.EntityFrameworkCore;
using PirateConquest.Database;
using PirateConquest.Models;

namespace PirateConquest.Repositories;

public class RoundRepository
{
    private readonly AppDbContext _context;

    public RoundRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Round?> GetCurrentRoundAsync()
    {
        var now = DateTime.UtcNow;
        return await _context
            .Rounds.OrderByDescending(round => round.StartPlanning)
            .FirstOrDefaultAsync(round => round.StartPlanning <= now);
    }

    public async Task<List<Round>> AllPlayableRoundsAsync() =>
        await _context
            .Rounds.Where(round => !round.IsInitial)
            .OrderBy(round => round.StartPlanning)
            .ToListAsync();

    public async Task<Round?> GetPreviousRoundAsync()
    {
        return await _context
            .Rounds.Where(round => _context.Outcomes.Any(outcome => outcome.RoundId == round.Id))
            .OrderByDescending(round => round.StartPlanning)
            .FirstOrDefaultAsync();
    }

    public async Task<Round?> RoundBeforeAsync(Round round) =>
        await _context
            .Rounds.OrderByDescending(roundBefore => roundBefore.End)
            .FirstOrDefaultAsync(roundBefore => roundBefore.End < round.End);

    public async Task<int> CountTeamShipsAsync(Sea sea, Team team, Round round = null)
    {
        round ??= await GetCurrentRoundAsync();
        var previousRound = await RoundBeforeAsync(round);
        var previousOutcome = await _context
            .Outcomes.Include(outcome => outcome.Round)
            .Include(outcome => outcome.Team)
            .Include(outcome => outcome.Sea)
            .FirstOrDefaultAsync(outcome =>
                outcome.Round.Id == previousRound.Id
                && outcome.Team.Id == team.Id
                && outcome.Sea.Id == sea.Id
            );
        var previousShipCount = previousOutcome?.ShipsAfter ?? 0;

        var shipsPurchasedAtSea = await _context
            .Purchases.Include(purchase => purchase.Round)
            .Include(purchase => purchase.Sea)
            .Include(purchase => purchase.Team)
            .Include(purchase => purchase.Round)
            .Where(purchase =>
                purchase.Round.Id == round.Id
                && purchase.Sea.Id == sea.Id
                && purchase.Team.Id == team.Id
            )
            .SumAsync(purchase => purchase.ShipCount);

        var shipsMovedToSea = await _context
            .Moves.Include(move => move.Round)
            .Include(move => move.Team)
            .Include(move => move.ToSea)
            .Where(move =>
                move.Round.Id == round.Id && move.ToSea.Id == sea.Id && move.Team.Id == team.Id
            )
            .SumAsync(move => move.ShipCount);

        var shipsMovedFromSea = await _context
            .Moves.Include(move => move.Round)
            .Include(move => move.Team)
            .Include(move => move.FromSea)
            .Where(move =>
                move.Round.Id == round.Id && move.FromSea.Id == sea.Id && move.Team.Id == team.Id
            )
            .SumAsync(move => move.ShipCount);

        var shipCount =
            previousShipCount + shipsPurchasedAtSea + shipsMovedToSea - shipsMovedFromSea;
        return shipCount;
    }
}
