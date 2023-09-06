using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CTFWhodunnit.Database;
using CTFWhodunnit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace CTFWhodunnit.Controllers;

[Authorize]
public class HintController : AuthenticatedController
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public HintController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _config = configuration;
    }

    public async Task<IActionResult> Index()
    {
        var ctfIdConfig = await _context.AppConfigs.FirstOrDefaultAsync(c => c.Name == AppConfig.CTF_ID_KEY);
        var teamsViewConfig = await _context.AppConfigs.FirstOrDefaultAsync(c => c.Name == AppConfig.TEAM_VIEW_KEY);

        if (ctfIdConfig == null || teamsViewConfig == null)
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        }

        int ctfId = int.Parse(ctfIdConfig.Value);
        bool teamsView = bool.Parse(teamsViewConfig.Value);

        int points = await FetchUserPointsAsync(User.Identity.Name, ctfId, teamsView);

        var hints = await _context.Hints.ToListAsync();

        foreach (var hint in hints)
        {
            if (hint.RequiredPoints > points)
            {
                hint.Text = null; // Clear the text if the user doesn't have enough points
            }
        }

        ViewData["Points"] = points;
        return View(hints);
    }

    public async Task<int> FetchUserPointsAsync(string currentUsername, int ctfEventId, bool teamView)
    {
        var playgroundLeaderboardUrl = await _context.AppConfigs.FirstOrDefaultAsync(c => c.Name == AppConfig.PLAYGROUND_LEADERBOARD_URL_KEY);
        if (playgroundLeaderboardUrl == null)
        {
            return 0;
        }

        using (HttpClient httpClient = new HttpClient())
        {
            string apiUrl = playgroundLeaderboardUrl.Value.Replace("$ctfEventId", ctfEventId.ToString()); 
            if (teamView)
            {
                apiUrl += "?teams=true";
            }

            HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string responseJson = await response.Content.ReadAsStringAsync();
                JObject responseObject = JObject.Parse(responseJson);

                var user = responseObject["items"]
                    .Children()
                    .FirstOrDefault(x => x["username"].ToString() == currentUsername);

                if (user != null)
                {
                    return (int)user["points"];
                }
            }
        }

        return 0; // Return null if user not found or API request failed
    }
}