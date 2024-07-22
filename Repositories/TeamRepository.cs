using CTFWhodunnit.Database;
using Microsoft.EntityFrameworkCore;
using PirateConquest.Models;

namespace PirateConquest.Repositories;

public class TeamRepository
{
    private readonly AppDbContext _context;

    public TeamRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Team>> All() =>
        await _context.Teams.Include(team => team.StartingSea).ToListAsync();

    public async Task<Team?> ByIdAsync(int? id)
    {
        if (id is null)
        {
            return null;
        }
        else
        {
            return await _context
                .Teams.Include(team => team.StartingSea)
                .FirstOrDefaultAsync(team => team.Id == id);
        }
    }
}
