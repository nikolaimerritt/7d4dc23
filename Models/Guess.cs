using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTFWhodunnit.Models;

public class Guess
{
    public int GuessId { get; set; }
    public int UserId { get; set; }
    public bool Correct { get; set; }
    public User User { get; set; }
    public Suspect Mastermind { get; set; }
    public DateTime TimeGuessed { get; set; }
}
