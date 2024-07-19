using PirateConquest.Models;

namespace PirateConquest.ViewModels;

public class MoveViewModel
{
    public int Id { get; set; }
    public RoundViewModel Round { get; set; }
    public TeamViewModel Team { get; set; }
    public SeaViewModel FromSea { get; set; }
    public SeaViewModel ToSea { get; set; }
    public int ShipCount { get; set; }
    public DateTime Creation { get; set; }

    public static MoveViewModel FromModel(Move move) =>
        new()
        {
            Id = move.Id,
            Round = RoundViewModel.FromModel(move.Round),
            Team = TeamViewModel.FromModel(move.Team),
            FromSea = SeaViewModel.FromModel(move.FromSea),
            ToSea = SeaViewModel.FromModel(move.ToSea),
            ShipCount = move.ShipCount,
            Creation = move.Creation,
        };
}
