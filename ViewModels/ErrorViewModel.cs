namespace PirateConquest.ViewModels;

public class ErrorViewModel
{
    public string Error { get; set; }

    public static readonly ErrorViewModel Unauthorized = new() { Error = "Unauthorized." };

    public static readonly ErrorViewModel MoveWindowHasEnded =
        new() { Error = "The movement window of the current round has ended." };

    public static readonly ErrorViewModel SeasAreInaccessible =
        new() { Error = "The seas are inaccessible." };

    public static readonly ErrorViewModel NotEnoughPoints =
        new() { Error = "You do not have enough points." };

    public static readonly ErrorViewModel NotEnoughShips =
        new() { Error = "You do not have enough ships." };
}
