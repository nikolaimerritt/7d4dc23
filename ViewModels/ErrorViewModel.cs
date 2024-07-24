namespace PirateConquest.ViewModels;

public class ErrorViewModel
{
    public string Message { get; set; }

    public static ErrorViewModel Unauthorized = new() { Message = "Unauthorized." };

    public static ErrorViewModel MoveWindowHasEnded =
        new() { Message = "The movement window of the current round has ended." };

    public static ErrorViewModel SeasAreInaccessible =
        new() { Message = "The seas are inaccessible." };

    public static ErrorViewModel NotEnoughPoints =
        new() { Message = "You do not have enough points." };

    public static ErrorViewModel NotEnoughShips =
        new() { Message = "You do not have enough ships." };
}
