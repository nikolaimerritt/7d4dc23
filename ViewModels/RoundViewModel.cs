using PirateConquest.Models;

namespace PirateConquest.ViewModels;

public class RoundViewModel
{
    public int Id { get; set; }
    public DateTime StartPlanning { get; set; }
    public DateTime StartCooldown { get; set; }
    public DateTime End { get; set; }

    public RoundViewModel() { }

    public static RoundViewModel FromModel(Round round) =>
        new()
        {
            Id = round.Id,
            StartPlanning = round.StartPlanning,
            StartCooldown = round.StartCooldown,
            End = round.End,
        };
}
