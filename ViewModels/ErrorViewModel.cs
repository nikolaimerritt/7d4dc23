namespace PirateConquest.ViewModels;

public class ErrorViewModel
{
    public string Error { get; set; }

    public static readonly ErrorViewModel Unauthorized = new() { Error = "Unauthorized." };

    public static readonly ErrorViewModel PlanningWindowHasEnded =
        new() { Error = "The round's planning window has ended." };

    public static readonly ErrorViewModel SeasAreInaccessible =
        new() { Error = "The seas are inaccessible." };

    public static readonly ErrorViewModel NotEnoughPoints =
        new() { Error = "You do not have enough points." };

    public static readonly ErrorViewModel NotEnoughShips =
        new() { Error = "You do not have enough ships." };
    public static readonly ErrorViewModel InternalError =
        new() { Error = "There was an internal error." };

    public static readonly ErrorViewModel InvalidMessageRecipient =
        new() { Error = "The message recipient is invalid." };
    public static readonly ErrorViewModel MessageTooLong =
        new() { Error = "The message is too long." };
    public static readonly ErrorViewModel TooManyMessages =
        new() { Error = "You have sent too many messages." };
}
