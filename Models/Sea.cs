using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PirateConquest.Models;

public class Sea
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }

    public static class Names
    {
        public static readonly string NorthPacific = "North Pacific";
        public static readonly string SouthPacific = "South Pacific";
        public static readonly string NorthAtlantic = "North Atlantic";
        public static readonly string SouthAtlantic = "South Atlantic";
        public static readonly string Southern = "Southern";
        public static readonly string Indian = "Indian";
        public static readonly string Arctic = "Arctic";

        public static readonly IReadOnlyList<string> All = new List<string>()
        {
            NorthPacific,
            SouthPacific,
            NorthAtlantic,
            SouthAtlantic,
            Southern,
            Indian,
            Arctic
        };

        public static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> AdjacentSeas =
            new Dictionary<string, IReadOnlyList<string>>()
            {
                [NorthPacific] = new List<string>() { SouthPacific },
                [SouthPacific] = new List<string>() { NorthPacific, Southern, SouthAtlantic },
                [Southern] = new List<string>() { SouthPacific, SouthAtlantic, Indian },
                [SouthAtlantic] = new List<string>() { SouthPacific, Southern, NorthAtlantic },
                [NorthAtlantic] = new List<string>() { SouthAtlantic, Arctic },
                [Arctic] = new List<string>() { NorthAtlantic },
                [Indian] = new List<string>() { Southern }
            };
    }
}
