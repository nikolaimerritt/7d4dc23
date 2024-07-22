using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PirateConquest.Models;

public class Round
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public bool IsInitial { get; set; } = false;
    public DateTime StartMoving { get; set; }
    public DateTime StartFighting { get; set; }
    public DateTime End { get; set; }
}
