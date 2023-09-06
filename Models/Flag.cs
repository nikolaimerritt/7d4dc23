using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTFWhodunnit.Models;

public class Flag
{
    public int FlagId { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
    public int Points { get; set; }
}