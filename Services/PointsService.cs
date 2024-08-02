using Newtonsoft.Json.Linq;
using PirateConquest.Models;
using static PirateConquest.Models.AppConfig;

namespace PirateConquest.Services;

public class PointsService
{
    private readonly ConfigService _configService;

    public PointsService(ConfigService configService)
    {
        _configService = configService;
    }

    public async Task<int?> GetPointsAsync(Team team)
    {
        var playgroundLeaderboardUrl = await _configService.GetValueAsync(
            StringConfig.PlaygroundLeaderboardUrl
        );
        if (playgroundLeaderboardUrl is string url)
        {
            using var httpClient = new HttpClient();
            var ctfEventId = await _configService.GetValueAsync(IntegerConfig.CtfIdKey);
            var apiUrl = url.Replace("$ctfEventId", ctfEventId.ToString());
            var response = await httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var responseObject = JObject.Parse(await response.Content.ReadAsStringAsync());
                var entries = responseObject["items"]?.Children();
                var teamEntry = entries?.FirstOrDefault(entry =>
                    entry["username"]?.ToString() == team.Name
                );
                if (teamEntry is JToken token)
                {
                    return int.TryParse(token["points"]?.ToString(), out var points)
                        ? points
                        : null;
                }
            }
        }
        return 0;
    }
}
