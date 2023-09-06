
using CTFWhodunnit.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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