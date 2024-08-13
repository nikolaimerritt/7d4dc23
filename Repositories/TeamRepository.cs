using Microsoft.EntityFrameworkCore;
using PirateConquest.Database;
using PirateConquest.Models;

namespace PirateConquest.Repositories;

public class TeamRepository
{
    private readonly AppDbContext _context;
    private readonly SeaRepository _seaRepository;

    public TeamRepository(AppDbContext context, SeaRepository seaRepository)
    {
        _context = context;
        _seaRepository = seaRepository;
    }

    public async Task<List<Team>> AllAsync()
    {
        var teams = await _context
            .Teams.Include(team => team.StartingSea)
            .OrderBy(team => team.Name)
            .ToListAsync();
        foreach (var team in teams)
        {
            team.StartingSea.AdjacentSeas = await _seaRepository.AdjacentSeasAsync(
                team.StartingSea
            );
        }
        return teams;
    }

    public async Task<Team?> ByIdAsync(int? teamId)
    {
        if (teamId is null)
        {
            return null;
        }

        var team = await _context
            .Teams.Include(team => team.StartingSea)
            .FirstOrDefaultAsync(team => team.Id == teamId);
        if (team is not null)
        {
            team.StartingSea.AdjacentSeas = await _seaRepository.AdjacentSeasAsync(
                team.StartingSea
            );
        }
        return team;
    }
}
