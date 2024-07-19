namespace PirateConquest.Models;

public class Round
{
    public int Id { get; set; }
    public DateTime StartMoving { get; set; }
    public DateTime StartFighting { get; set; }
    public DateTime End { get; set; }
}
