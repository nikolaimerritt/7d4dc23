namespace PirateConquest.Models;

public class Outcome
{
    public int Id { get; set; }
    public Round Round { get; set; }
    public Team Team { get; set; }
    public Sea Sea { get; set; }
    public int ShipCount { get; set; }
}
