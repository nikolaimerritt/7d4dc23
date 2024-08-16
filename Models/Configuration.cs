namespace PirateConquest.Models;

public class Configuration
{
    public int Id { get; set; }
    public string PlaygroundLeaderboardUrl { get; set; }
    public int CtfId { get; set; }
    public int RoundsCount { get; set; }
    public int PlanningMinutes { get; set; }
    public int CooldownMinutes { get; set; }
    public DateTime FirstRoundStartUtc { get; set; }
    public int MaxMessageCharacters { get; set; }
    public int MaxMessagesPerTeam { get; set; }
}
