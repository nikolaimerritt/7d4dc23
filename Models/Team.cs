using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PirateConquest.Models;

public class Team
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }
    public string PlainTextPassword { get; set; }
    public string ColourHexCode { get; set; }
    public Sea StartingSea { get; set; }
}
