using Microsoft.EntityFrameworkCore;
using PirateConquest.Database;
using PirateConquest.Models;

namespace PirateConquest.Repositories;

public class ConfigRepository
{
    private readonly AppDbContext _context;

    public ConfigRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string?> GetValueAsync(AppConfig.StringConfig config) =>
        await GetValueAsync(config.Key);

    public async Task<int?> GetValueAsync(AppConfig.IntegerConfig config)
    {
        var value = await GetValueAsync(config.Key);
        return int.TryParse(value, out var intValue) ? intValue : null;
    }

    public async Task<DateTime?> GetValueAsync(AppConfig.DateTimeConfig config)
    {
        var value = await GetValueAsync(config.Key);
        return DateTime.TryParse(value, out var dateTimeValue) ? dateTimeValue : null;
    }

    private async Task<string?> GetValueAsync(string key)
    {
        var config = await _context.AppConfigs.FirstOrDefaultAsync(config => config.Key == key);
        return config?.Value;
    }
}
