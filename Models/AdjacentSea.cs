using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PirateConquest.Models;

public class AdjacentSea
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public Sea Sea { get; set; }
    public Sea AdjacentTo { get; set; }
}
