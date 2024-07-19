using PirateConquest.Models;

namespace PirateConquest.ViewModels;

public class PurchaseViewModel
{
    public int Id { get; set; }
    public TeamViewModel Team { get; set; }
    public RoundViewModel Round { get; set; }
    public SeaViewModel Sea { get; init; }
    public int Points { get; set; }
    public int ShipCount { get; set; }
    public DateTime Creation { get; set; }

    public static PurchaseViewModel FromModel(Purchase purchase) =>
        new()
        {
            Id = purchase.Id,
            Team = TeamViewModel.FromModel(purchase.Team),
            Round = RoundViewModel.FromModel(purchase.Round),
            Sea = SeaViewModel.FromModel(purchase.Sea),
            Points = purchase.Points,
            ShipCount = purchase.ShipCount,
            Creation = purchase.Creation,
        };
}
