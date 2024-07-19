namespace PirateConquest.Models;

public class Purchase
{
    public int Id { get; set; }
    public Team Team { get; set; }
    public Round Round { get; set; }
    public int Points { get; set; }
    public int ShipCount { get; set; }
    public DateTime Creation { get; set; }
}
