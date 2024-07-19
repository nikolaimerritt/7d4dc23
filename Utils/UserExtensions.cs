using System.Security.Claims;
using CTFWhodunnit.Database;
using Microsoft.EntityFrameworkCore;
using PirateConquest.Models;

namespace PirateConquest.Utils;

public static class UserExtensions
{
    public static async Task<Team?> GetTeamAsync(this ClaimsPrincipal user, AppDbContext context)
    {
        if (!user.Identity?.IsAuthenticated ?? false)
        {
            return null;
        }
        else
        {
            var sidClaim = user.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid);
            if (int.TryParse(sidClaim.Value, out int teamId))
            {
                return await context.Teams.FirstOrDefaultAsync(team => team.Id == teamId);
            }
            else
            {
                return null;
            }
        }
    }
}
