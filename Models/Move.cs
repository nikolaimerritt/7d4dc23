using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PirateConquest.Models;

public class Move
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int RoundId { get; set; }
    public Round Round { get; set; }
    public int TeamId { get; set; }
    public Team Team { get; set; }
    public int FromSeaId { get; set; }
    public Sea FromSea { get; set; }
    public int ToSeaId { get; set; }
    public Sea ToSea { get; set; }
    public int ShipCount { get; set; }
    public DateTime Creation { get; set; }
}
