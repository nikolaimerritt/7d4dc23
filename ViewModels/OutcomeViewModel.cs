using PirateConquest.Models;

namespace PirateConquest.ViewModels;

public class OutcomeViewModel
{
    public int? Id { get; set; }
    public RoundViewModel Round { get; set; }
    public TeamViewModel Team { get; set; }
    public SeaViewModel Sea { get; set; }
    public int ShipCount { get; set; }

    public static OutcomeViewModel FromModel(Outcome outcome) =>
        new()
        {
            Id = outcome.Id,
            Round = RoundViewModel.FromModel(outcome.Round),
            Team = TeamViewModel.FromModel(outcome.Team),
            Sea = SeaViewModel.FromModel(outcome.Sea),
            ShipCount = outcome.ShipCount
        };
}
