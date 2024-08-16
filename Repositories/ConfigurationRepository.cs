using Microsoft.EntityFrameworkCore;
using PirateConquest.Database;
using PirateConquest.Models;

namespace PirateConquest.Repositories;

public class ConfigurationRepository
{
    private readonly AppDbContext _context;
    private static readonly Configuration Default =
        new()
        {
            PlaygroundLeaderboardUrl = "",
            CtfId = -1,
            RoundsCount = -1,
            PlanningMinutes = -1,
            CooldownMinutes = -1,
            FirstRoundStartUtc = DateTime.MinValue,
            MaxMessageCharacters = 250,
            MaxMessagesPerTeam = 1000
        };

    public ConfigurationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task WriteDefaultAsync()
    {
        if (!await _context.Configurations.AnyAsync())
        {
            await _context.Configurations.AddAsync(Default);
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
        configuration.PlaygroundLeaderboardUrl == Default.PlaygroundLeaderboardUrl
        || configuration.CtfId == Default.CtfId
        || configuration.RoundsCount == Default.RoundsCount
        || configuration.PlanningMinutes == Default.PlanningMinutes
        || configuration.CooldownMinutes == Default.CooldownMinutes
        || configuration.FirstRoundStartUtc == Default.FirstRoundStartUtc;
}
