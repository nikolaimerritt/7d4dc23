namespace PirateConquest.Models;

public class AdjacentSea
{
    public int Id { get; set; }
    public Sea Sea { get; set; }
    public Sea AdjacentTo { get; set; }
}
