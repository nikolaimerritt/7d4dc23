using PirateConquest.Models;

namespace PirateConquest.ViewModels;

public class MessageViewModel
{
    public int Id { get; set; }
    public TeamViewModel Sender { get; set; }
    public TeamViewModel Recipient { get; set; }
    public string Content { get; set; }
    public bool ReadByRecipient { get; set; }
    public DateTime Creation { get; set; }

    public static MessageViewModel FromModel(Message message) =>
        new()
        {
            Id = message.Id,
            Sender = TeamViewModel.FromModel(message.Sender),
            Recipient = TeamViewModel.FromModel(message.Recipient),
            Content = message.Content,
            ReadByRecipient = message.ReadByRecipient,
            Creation = message.Creation,
        };
}
