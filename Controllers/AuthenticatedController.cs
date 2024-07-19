using System.Security.Claims;
using CTFWhodunnit.Models;
using Microsoft.AspNetCore.Mvc;

namespace CTFWhodunnit.Controllers;

public class AuthenticatedController : Controller
{
    protected int? LoggedInUserId
    {
        get
        {
            if (User.Identity.IsAuthenticated)
            {
                return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            return null;
        }
    }
}
