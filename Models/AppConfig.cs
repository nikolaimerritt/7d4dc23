using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTFWhodunnit.Models;
public class AppConfig
{

    public static readonly string CTF_ID_KEY = "CtfId";
    public static readonly string TEAM_VIEW_KEY = "TeamView";
    public static readonly string VIDEO_URL_KEY = "VideoUrl";
    public static readonly string PLAYGROUND_LEADERBOARD_URL_KEY = "PlaygroundLeaderboardUrl";
    public static readonly string MAX_FLAG_POINTS_KEY = "MaxFlagPoints";
    public static readonly string MAX_CTF_POINTS_KEY = "MaxCtfPoints";

    public int Id { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
}