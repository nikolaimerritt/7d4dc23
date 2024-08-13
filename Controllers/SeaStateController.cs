using Microsoft.AspNetCore.Mvc;
using PirateConquest.Models;
using PirateConquest.Repositories;
using PirateConquest.ViewModels;

namespace PirateConquest.Controllers;

public class SeaStateController : Controller
{
    private readonly OutcomeRepository _outcomeRepository;
    private readonly TeamRepository _teamRepository;
    private readonly MoveRepository _moveRepository;
    private readonly RoundRepository _roundRepository;
    private readonly PurchaseRepository _purchaseRepository;
    private readonly SeaRepository _sealRepository;

    public SeaStateController(
        OutcomeRepository outcomeRepository,
        TeamRepository teamRepository,
        MoveRepository moveRepository,
        RoundRepository roundRepository,
        PurchaseRepository purchaseRepository,
        SeaRepository seaRepository
    )
    {
        _outcomeRepository = outcomeRepository;
        _teamRepository = teamRepository;
        _moveRepository = moveRepository;
        _roundRepository = roundRepository;
        _purchaseRepository = purchaseRepository;
        _sealRepository = seaRepository;
    }

    [HttpGet("/api/sea-states")]
    public async Task<IActionResult> GetSeaStates()
    {
        var now = DateTime.UtcNow;
        var round = await _roundRepository.GetCurrentRoundAsync();
        var outcomes = await _outcomeRepository.InPreviousRoundAsync();
        var seaStates = await GetStateFromOutcomes(outcomes);
        if (round.StartPlanning <= now && now < round.StartCooldown)
        {
            await AddCurrentPurchases(round, seaStates);
            await AddCurrentMoves(round, seaStates);
        }

        return Json(seaStates);
    }

    private async Task<List<SeaStateViewModel>> GetStateFromOutcomes(List<Outcome> outcomes)
    {
        var seas = await _sealRepository.AllAsync();
        var seaStates = new List<SeaStateViewModel>(seas.Count);
        foreach (var sea in seas)
        {
            var teamShips = outcomes
                .Where(outcome => outcome.Sea.Id == sea.Id)
                .Select(outcome => new TeamShipCountViewModel()
                {
                    Team = TeamViewModel.FromModel(outcome.Team),
                    ShipCount = outcome.ShipsAfter
                })
                .ToList();
            var seaState = seaStates.Find(seaState => seaState.Sea.Id == sea.Id);
            if (seaState is null)
            {
                seaStates.Add(new() { Sea = SeaViewModel.FromModel(sea), TeamShips = teamShips });
            }
            else
            {
                seaState.TeamShips.AddRange(teamShips);
            }
        }
        return seaStates;
    }

    private async Task AddCurrentPurchases(Round currentRound, List<SeaStateViewModel> seaStates)
    {
        foreach (var purchase in await _purchaseRepository.FromRoundAsync(currentRound))
        {
            var seaState = seaStates.Find(seaState => seaState.Sea.Id == purchase.Sea.Id);
            if (seaState is null)
            {
                seaStates.Add(
                    new()
                    {
                        Sea = SeaViewModel.FromModel(purchase.Sea),
                        TeamShips = new()
                        {
                            new()
                            {
                                Team = TeamViewModel.FromModel(purchase.Team),
                                ShipCount = purchase.ShipCount
                            }
                        }
                    }
                );
            }
            else
            {
                var teamShips = seaState.TeamShips.Find(teamShips =>
                    teamShips.Team.Id == purchase.Team.Id
                );
                if (teamShips is null)
                {
                    seaState.TeamShips.Add(
                        new()
                        {
                            Team = TeamViewModel.FromModel(purchase.Team),
                            ShipCount = purchase.ShipCount
                        }
                    );
                }
                else
                {
                    teamShips.ShipCount += purchase.ShipCount;
                }
            }
        }
    }

    private async Task AddCurrentMoves(Round currentRound, List<SeaStateViewModel> seaStates)
    {
        foreach (var move in await _moveRepository.FromRoundAsync(currentRound))
        {
            var stateInExitSea = seaStates.Find(seaState => seaState.Sea.Id == move.FromSea.Id);
            if (stateInExitSea is not null)
            {
                var teamShips = stateInExitSea.TeamShips.Find(teamShips =>
                    teamShips.Team.Id == move.Team.Id
                );
                if (teamShips is not null)
                {
                    teamShips.ShipCount -= move.ShipCount;
                }
            }

            var stateInEntranceSea = seaStates.Find(seaState => seaState.Sea.Id == move.ToSea.Id);
            if (stateInEntranceSea is null)
            {
                seaStates.Add(
                    new()
                    {
                        Sea = SeaViewModel.FromModel(move.ToSea),
                        TeamShips = new()
                        {
                            new()
                            {
                                Team = TeamViewModel.FromModel(move.Team),
                                ShipCount = move.ShipCount
                            }
                        }
                    }
                );
            }
            else
            {
                var teamShips = stateInEntranceSea.TeamShips.Find(teamShips =>
                    teamShips.Team.Id == move.Team.Id
                );
                if (teamShips is not null)
                {
                    teamShips.ShipCount += move.ShipCount;
                }
            }
        }
    }
}
