using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTFWhodunnit.Models;

public class UnlockedIntel
{
    public int UnlockedIntelId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public Suspect Suspect { get; set; }
    public DateTime TimeUnlocked { get; set; }
}
