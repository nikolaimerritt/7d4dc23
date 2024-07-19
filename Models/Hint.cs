using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTFWhodunnit.Models;

public class Hint
{
    public int HintId { get; set; }
    public string Name { get; set; }
    public string Text { get; set; }
    public int RequiredPoints { get; set; }
}
