using Microsoft.EntityFrameworkCore;
using PirateConquest.Database;
using PirateConquest.Models;

namespace PirateConquest.Repositories;

public class ConfigurationRepository
{
    private readonly AppDbContext _context;
    private static readonly Configuration Empty =
        new()
        {
            PlaygroundLeaderboardUrl = "",
            CtfId = -1,
            RoundsCount = -1,
            PlanningMinutes = -1,
            CooldownMinutes = -1,
            FirstRoundStartUtc = DateTime.MinValue
        };

    public ConfigurationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task WriteEmptyAsync()
    {
        if (!await _context.Configurations.AnyAsync())
        {
            await _context.Configurations.AddAsync(Empty);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Configuration?> GetNonEmptyAsync()
    {
        var configuration = await _context
            .Configurations.OrderBy(config => config.Id)
            .FirstOrDefaultAsync();
        if (configuration is null || HasEmptyField(configuration))
        {
            return null;
        }
        else
        {
            return configuration;
        }
    }

    private static bool HasEmptyField(Configuration configuration) =>
        configuration.PlaygroundLeaderboardUrl == Empty.PlaygroundLeaderboardUrl
        || configuration.CtfId == Empty.CtfId
        || configuration.RoundsCount == Empty.RoundsCount
        || configuration.PlanningMinutes == Empty.PlanningMinutes
        || configuration.CooldownMinutes == Empty.CooldownMinutes
        || configuration.FirstRoundStartUtc == Empty.FirstRoundStartUtc;
}
