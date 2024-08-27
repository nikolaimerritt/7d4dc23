using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PirateConquest.Models;

public class Message
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int SenderId { get; set; }
    public Team Sender { get; set; }
    public int RecipientId { get; set; }
    public Team Recipient { get; set; }
    public string Content { get; set; }
    public bool ReadByRecipient { get; set; }
    public DateTime Creation { get; set; }
}
