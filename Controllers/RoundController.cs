using Microsoft.AspNetCore.Mvc;
using PirateConquest.Repositories;
using PirateConquest.ViewModels;

namespace PirateConquest.Controllers;

public class RoundController : Controller
{
    private readonly RoundRepository _roundRepository;

    public RoundController(RoundRepository roundRepository)
    {
        _roundRepository = roundRepository;
    }

    [HttpGet("/api/rounds")]
    public async Task<IActionResult> GetRounds()
    {
        var rounds = await _roundRepository.AllPlayableRoundsAsync();
        return Json(rounds.Select(RoundViewModel.FromModel));
    }
}
