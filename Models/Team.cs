using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PirateConquest.Models;

public class Team
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }

    [JsonIgnore]
    public string PlainTextPassword { get; set; }
    public Sea StartingSea { get; set; }
}
