using Microsoft.EntityFrameworkCore;
using PirateConquest.Database;
using PirateConquest.Models;

namespace PirateConquest.Services;

public class ConfigService
{
    private readonly AppDbContext _context;

    public ConfigService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string?> GetValueAsync(AppConfig.StringConfig configKey) =>
        await GetValueAsync(configKey.Key);

    public async Task<int?> GetValueAsync(AppConfig.IntegerConfig configKey)
    {
        var value = await GetValueAsync(configKey.Key);
        return int.TryParse(value, out var intValue) ? intValue : null;
    }

    public async Task<DateTime?> GetValueAsync(AppConfig.DateTimeConfig configKey)
    {
        var value = await GetValueAsync(configKey.Key);
        return DateTime.TryParse(value, out var dateTimeValue) ? dateTimeValue : null;
    }

    private async Task<string?> GetValueAsync(string key)
    {
        var environmentVariable = Environment.GetEnvironmentVariable(key);
        if (environmentVariable is string value)
        {
            return value;
        }
        else
        {
            var config = await _context.AppConfigs.FirstOrDefaultAsync(config => config.Key == key);
            return config?.Value;
        }
    }
}
