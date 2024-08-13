using Microsoft.EntityFrameworkCore;
using PirateConquest.Database;
using PirateConquest.Models;
using SQLitePCL;

namespace PirateConquest.Repositories;

public class MoveRepository
{
    private readonly AppDbContext _context;

    public MoveRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Move>> AllAsync() =>
        await _context
            .Moves.Include(move => move.FromSea)
            .Include(move => move.ToSea)
            .Include(move => move.Round)
            .Include(move => move.Team)
            .ThenInclude(team => team.StartingSea)
            .OrderBy(move => move.ToSea.Name)
            .ThenBy(move => move.Team.Name)
            .ToListAsync();

    public async Task<List<Move>> FromRoundAsync(Round round) =>
        (await AllAsync()).Where(move => move.Round.Id == round.Id).ToList();

    public async Task<bool> AnyInRoundAsync(Team team, Round round) =>
        await _context
            .Moves.Include(move => move.Round)
            .Include(move => move.Team)
            .AnyAsync(move => move.TeamId == team.Id && move.RoundId == round.Id);

    public async Task AddIfNotExistsAsync(Move move)
    {
        var moveToAdd = new Move()
        {
            Id = move.Id,
            RoundId = move.Round.Id,
            TeamId = move.Team.Id,
            FromSeaId = move.FromSea.Id,
            ToSeaId = move.ToSea.Id,
            ShipCount = move.ShipCount,
            Creation = move.Creation
        };
        if (
            !await _context.Moves.AnyAsync(move =>
                move.RoundId == moveToAdd.RoundId
                && move.TeamId == moveToAdd.TeamId
                && move.FromSeaId == moveToAdd.FromSeaId
                && move.ToSeaId == moveToAdd.ToSeaId
                && move.ShipCount == moveToAdd.ShipCount
            )
        )
        {
            await _context.Moves.AddAsync(moveToAdd);
            await _context.SaveChangesAsync();
        }
    }
}
