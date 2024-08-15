using Hangfire;
using Microsoft.AspNetCore.Mvc;
using PirateConquest.Models;
using PirateConquest.Repositories;
using PirateConquest.Utils;
using PirateConquest.ViewModels;

namespace PirateConquest.Controllers;

public class MessageController : Controller
{
    private readonly MessageRepository _messageRepository;
    private readonly TeamRepository _teamRepository;

    public MessageController(MessageRepository messageRepository, TeamRepository teamRepository)
    {
        _messageRepository = messageRepository;
        _teamRepository = teamRepository;
    }

    [HttpGet("/api/messages")]
    public async Task<IActionResult> GetMessages()
    {
        var recipient = await _teamRepository.ByIdAsync(User.GetTeamId());
        if (recipient is null)
        {
            return Json(ErrorViewModel.Unauthorized);
        }
        else
        {
            return Json(await _messageRepository.AllToRecipientAsync(recipient));
        }
    }

    [HttpPost("/api/messages")]
    public async Task<IActionResult> WriteMessage(
        [FromParameter("teamId")] int toTeamId,
        [FromBody] string messageContent
    )
    {
        var sender = await _teamRepository.ByIdAsync(User.GetTeamId());
        if (sender is null)
        {
            return Json(ErrorViewModel.Unauthorized);
        }
        var recipient = await _teamRepository.ByIdAsync(toTeamId);
        if (recipient is null || recipient.Id == sender.Id)
        {
            return Json(ErrorViewModel.InvalidMessageRecipient);
        }

        var message = new Message()
        {
            Sender = sender,
            Recipient = recipient,
            Read = false,
            Content = messageContent,
            Creation = DateTime.UtcNow
        };
        await _messageRepository.AddAsync(message);
        return Json(new OkViewModel());
    }

    [HttpPost("/api/messages/read")]
    public async Task<IActionResult> MarkMessagesAsRead([FromBody] int[] readMessageIds)
    {
        var recipient = await _teamRepository.ByIdAsync(User.GetTeamId());
        if (recipient is null)
        {
            return Json(ErrorViewModel.Unauthorized);
        }
        await _messageRepository.MarkAsReadAsync(recipient, readMessageIds);
        return Json(new OkViewModel());
    }
}
