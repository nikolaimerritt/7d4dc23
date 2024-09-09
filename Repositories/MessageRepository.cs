using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using PirateConquest.Database;
using PirateConquest.Models;

namespace PirateConquest.Repositories;

public class MessageRepository
{
    private readonly AppDbContext _context;

    public MessageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Message>> AllBetweenAsync(Team one, Team two) =>
        await MessagesBetween(one, two).OrderBy(message => message.Creation).ToListAsync();

    public async Task<int> CountMessagesFromAsync(Team sender, Team recipient) =>
        await _context
            .Messages.Where(message =>
                message.Sender.Id == sender.Id && message.Recipient.Id == recipient.Id
            )
            .CountAsync();

    public async Task<int> CountUnreadMessagesAsync(Team sender, Team recipient) =>
        await MessageFromTo(sender, recipient)
            .Where(message => !message.ReadByRecipient)
            .CountAsync();

    private IQueryable<Message> MessagesBetween(Team one, Team two) =>
        _context
            .Messages.Include(message => message.Sender)
            .Include(message => message.Recipient)
            .Where(message =>
                message.Sender.Id == one.Id && message.Recipient.Id == two.Id
                || message.Recipient.Id == one.Id && message.Sender.Id == two.Id
            );

    private IQueryable<Message> MessageFromTo(Team sender, Team recipient) =>
        _context
            .Messages.Include(message => message.Sender)
            .Include(message => message.Recipient)
            .Where(message =>
                message.Sender.Id == sender.Id && message.Recipient.Id == recipient.Id
            );

    public async Task AddAsync(Message message)
    {
        var messageToAdd = new Message()
        {
            SenderId = message.Sender.Id,
            RecipientId = message.Recipient.Id,
            Content = message.Content,
            ReadByRecipient = message.ReadByRecipient,
            Creation = message.Creation,
        };
        await _context.Messages.AddAsync(messageToAdd);
        await _context.SaveChangesAsync();
    }

    public async Task MarkAsReadAsync(Team recipient, IEnumerable<int> messageIds)
    {
        await _context
            .Messages.Where(message =>
                message.Recipient.Id == recipient.Id
                && messageIds.Contains(message.Id)
                && !message.ReadByRecipient
            )
            .ExecuteUpdateAsync(message =>
                message.SetProperty(message => message.ReadByRecipient, true)
            );
    }
}
