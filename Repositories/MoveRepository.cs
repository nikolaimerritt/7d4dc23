using CTFWhodunnit.Database;
using Microsoft.EntityFrameworkCore;
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

    public async Task<List<Move>> All() =>
        await _context
            .Moves.Include(move => move.FromSea)
            .Include(move => move.ToSea)
            .Include(move => move.Round)
            .Include(move => move.Team)
            .ThenInclude(team => team.StartingSea)
            .ToListAsync();
}
