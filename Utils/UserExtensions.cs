using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PirateConquest.Database;
using PirateConquest.Models;
using PirateConquest.Repositories;

namespace PirateConquest.Utils;

public static class UserExtensions
{
    public static int? GetTeamId(this ClaimsPrincipal user)
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
                return teamId;
            }
            else
            {
                return null;
            }
        }
    }

    //public static async Task<Team?> GetTeamAsync(this ClaimsPrincipal user, TeamRepository teamRepository)
    //{
    //    if (!user.Identity?.IsAuthenticated ?? false)
    //    {
    //        return null;
    //    }
    //    else
    //    {
    //        var sidClaim = user.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid);
    //        if (int.TryParse(sidClaim.Value, out int teamId))
    //        {
    //            return (await teamRepository.All()).FirstOrDefault(team => team.Id == teamId);
    //        }
    //        else
    //        {
    //            return null;
    //        }
    //    }
    //}
}
