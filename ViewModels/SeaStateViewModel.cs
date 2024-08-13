namespace PirateConquest.ViewModels;

public class SeaStateViewModel
{
    public SeaViewModel Sea { get; set; }
    public List<TeamShipCountViewModel> TeamShips { get; set; }
}

public class TeamShipCountViewModel
{
    public TeamViewModel Team { get; set; }
    public int ShipCount { get; set; }
}
