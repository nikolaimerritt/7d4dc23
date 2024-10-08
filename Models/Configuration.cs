﻿namespace PirateConquest.Models;

public class Configuration
{
    public int Id { get; set; }
    public string PlaygroundLeaderboardUrl { get; set; }
    public int CtfId { get; set; }
    public int RoundsCount { get; set; }
    public int PlanningMinutes { get; set; }
    public int CooldownMinutes { get; set; }
    public DateTime FirstRoundStartUtc { get; set; }

    // NOTE: this does not propagate to the frontend.
    // Update the frontend if you are changing this.
    public int MaxMessageCharacters { get; set; }
    public int MaxMessagesPerTeam { get; set; }
    public int TeamStartingShips { get; set; }

    // NOTE: this does not propagate to the frontend.
    // Update the frontend if you are changing this.
    public int PointsPerShip { get; set; }

    public bool DebugAssignMaxPoints { get; set; }
}
