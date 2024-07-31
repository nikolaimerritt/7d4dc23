using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PirateConquest.Models;

public class Outcome
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int RoundId { get; set; }
    public Round Round { get; set; }
    public int TeamId { get; set; }
    public Team Team { get; set; }
    public int SeaId { get; set; }
    public Sea Sea { get; set; }
    public int ShipsBefore { get; set; }
    public int ShipsAfter { get; set; }
}
