using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTFWhodunnit.Models;

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; } // Store hashed & salted passwords for security
    public int Points { get; set; }
    public bool IsAdmin { get; set; }
}
