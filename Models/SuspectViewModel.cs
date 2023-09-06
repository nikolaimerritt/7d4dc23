using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTFWhodunnit.Models;
public class SuspectViewModel

{

    public int SuspectId { get; set; }

    public string Name { get; set; }

    public string Location { get; set; }

    public string OperatingSystem { get; set; }

    public string Skills { get; set; }

    public string Secret { get; set; }

}