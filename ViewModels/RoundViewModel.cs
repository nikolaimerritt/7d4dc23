using PirateConquest.Models;

namespace PirateConquest.ViewModels;

public class RoundViewModel
{
    public int Id { get; set; }
    public DateTime StartMoving { get; set; }
    public DateTime StartFighting { get; set; }
    public DateTime End { get; set; }

    public RoundViewModel() { }

    public static RoundViewModel FromModel(Round round) =>
        new()
        {
            Id = round.Id,
            StartMoving = round.StartMoving,
            StartFighting = round.StartFighting,
            End = round.End,
        };
}
