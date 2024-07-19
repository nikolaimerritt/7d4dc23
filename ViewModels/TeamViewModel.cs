using PirateConquest.Models;

namespace PirateConquest.ViewModels;

public class TeamViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ColourHexCode { get; set; }
    public SeaViewModel StartingSea { get; set; }

    public static TeamViewModel FromModel(Team team) =>
        new()
        {
            Id = team.Id,
            Name = team.Name,
            ColourHexCode = team.ColourHexCode,
            StartingSea = SeaViewModel.FromModel(team.StartingSea)
        };
}
