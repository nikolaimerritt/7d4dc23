using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PirateConquest.Models;

public class Purchase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public Team Team { get; set; }
    public Round Round { get; set; }
    public Sea Sea { get; init; }
    public int Points { get; set; }
    public int ShipCount { get; set; }
    public DateTime Creation { get; set; }
}
