using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PirateConquest.Models;

public class AppConfig
{
    public class StringConfig
    {
        public string Key { get; private init; }

        private StringConfig(string key)
        {
            Key = key;
        }

        public static readonly StringConfig PlaygroundLeaderboardUrl =
            new("PlaygroundLeaderboardUrl");
    }

    public class IntegerConfig
    {
        public string Key { get; private init; }

        private IntegerConfig(string key)
        {
            Key = key;
        }

        public static readonly IntegerConfig CtfIdKey = new("CtfId");
        public static readonly IntegerConfig RoundsCount = new("RoundsCount");
        public static readonly IntegerConfig PlanningMinutes = new("PlanningMinutes");
        public static readonly IntegerConfig CooldownMinutes = new("CooldownMinutes");
    }

    public class DateTimeConfig
    {
        public string Key { get; private set; }

        private DateTimeConfig(string key)
        {
            Key = key;
        }

        public static readonly DateTimeConfig FirstRoundStart = new("FirstRoundStart");
    }

    public int Id { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
}
