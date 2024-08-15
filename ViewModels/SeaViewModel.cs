using PirateConquest.Models;

namespace PirateConquest.ViewModels;

public class SeaViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<SeaViewModel> AdjacentSeas { get; set; }

    public static SeaViewModel FromModel(Sea sea) =>
        new()
        {
            Id = sea.Id,
            Name = sea.Name,
            AdjacentSeas =
                sea.AdjacentSeas?.Select(sea => new SeaViewModel() { Id = sea.Id, Name = sea.Name })
                    .ToList() ?? new()
        };
}
