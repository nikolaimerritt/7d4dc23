namespace PirateConquest.Models;

public class Team
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string PlainTextPassword { get; set; }
    public string ColourHexCode { get; set; }
    public Sea StartingSea { get; set; }
}
