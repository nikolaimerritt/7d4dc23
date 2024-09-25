using Newtonsoft.Json.Linq;
using PirateConquest.Models;
using PirateConquest.Repositories;

namespace PirateConquest.Services;

public class PointsService
{
    private readonly ConfigurationRepository _configurationRepository;

    public PointsService(ConfigurationRepository configurationRepository)
    {
        _configurationRepository = configurationRepository;
    }

    public async Task<int> GetPointsAsync(Team team)
    {
        var configuration = await _configurationRepository.GetNonEmptyAsync();
        if (configuration is not null)
        {
            using var httpClient = new HttpClient();
            var apiUrl = configuration.PlaygroundLeaderboardUrl.Replace(
                "$ctfEventId",
                configuration.CtfId.ToString()
            );
            var response = await httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var responseObject = JObject.Parse(await response.Content.ReadAsStringAsync());
                var entries = responseObject["items"]?.Children();
                var teamEntry = entries?.FirstOrDefault(entry =>
                    entry["username"]?.ToString() == team.Name
                );
                if (
                    teamEntry is JToken token
                    && int.TryParse(token["points"]?.ToString(), out var points)
                )
                {
                    return points;
                }
            }
        }
        return 0;
    }
}
