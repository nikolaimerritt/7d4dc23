using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PirateConquest.Models;

public class AppConfig
{
    public string PlaygroundLeaderboardUrl { get; set; }
    public int CtfId { get; set; }
    public int RoundsCount { get; set; }
    public int PlanningMinutes { get; set; }
    public int CooldownMinutes { get; set; }
    public DateTime FirstRoundStart { get; set; }

    public class StringConfig
    {
        public string Key { get; private init; }

        public static readonly StringConfig PlaygroundLeaderboardUrl =
            new() { Key = "PlaygroundLeaderboardUrl" };
    }

    public class IntegerConfig
    {
        public string Key { get; private init; }

        public static readonly IntegerConfig CtfIdKey = new() { Key = "CtfId" };
        public static readonly IntegerConfig RoundsCount = new() { Key = "RoundsCount" };
        public static readonly IntegerConfig PlanningMinutes = new() { Key = "PlanningMinutes" };
        public static readonly IntegerConfig CooldownMinutes = new() { Key = "CooldownMinutes" };
    }

    public class DateTimeConfig
    {
        public string Key { get; private set; }

        public static readonly DateTimeConfig FirstRoundStart =
            new() { Key = "FirstRoundStartUtc" };
    }

    public int Id { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
}
