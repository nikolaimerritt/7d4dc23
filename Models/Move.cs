namespace PirateConquest.Models;

public class Move
{
    public int Id { get; set; }
    public Round Round { get; set; }
    public Team Team { get; set; }
    public Sea FromSea { get; set; }
    public Sea ToSea { get; set; }
    public int ShipCount { get; set; }
    public DateTime Creation { get; set; }
}
