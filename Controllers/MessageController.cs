﻿using System.Net.Mime;
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
    private readonly ConfigurationRepository _configurationRepository;

    public MessageController(
        MessageRepository messageRepository,
        TeamRepository teamRepository,
        ConfigurationRepository configurationRepository
    )
    {
        _messageRepository = messageRepository;
        _teamRepository = teamRepository;
        _configurationRepository = configurationRepository;
    }

    [HttpGet("/api/messages")]
    public async Task<IActionResult> GetMessages(int withTeamId)
    {
        var teamOne = await _teamRepository.ByIdAsync(User.GetTeamId());
        if (teamOne is null)
        {
            return Json(ErrorViewModel.Unauthorized);
        }
        var teamTwo = await _teamRepository.ByIdAsync(withTeamId);
        if (teamTwo is null)
        {
            return Json(ErrorViewModel.InvalidMessageRecipient);
        }
        var messages = await _messageRepository.AllBetweenAsync(teamTwo, teamOne);
        return Json(messages.Select(MessageViewModel.FromModel));
    }

    [HttpGet("api/messages/notifications")]
    public async Task<IActionResult> GetUnreadNotifications()
    {
        var recipient = await _teamRepository.ByIdAsync(User.GetTeamId());
        if (recipient is null)
        {
            return Json(ErrorViewModel.Unauthorized);
        }
        var unreadMessages = new List<UnreadNotificationViewModel>();
        foreach (var sender in await _teamRepository.AllAsync())
        {
            if (sender.Id != recipient.Id)
            {
                var unreadMessagesCount = await _messageRepository.CountUnreadMessagesAsync(
                    sender,
                    recipient
                );
                if (unreadMessagesCount > 0)
                {
                    unreadMessages.Add(
                        new()
                        {
                            Sender = TeamViewModel.FromModel(sender),
                            UnreadMessagesCount = unreadMessagesCount
                        }
                    );
                }
            }
        }

        return Json(unreadMessages);
    }

    [HttpPost("/api/messages")]
    [Consumes(MediaTypeNames.Text.Plain)]
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
        var config = await _configurationRepository.GetNonEmptyAsync();
        if (messageContent.Length > config.MaxMessageCharacters)
        {
            return Json(ErrorViewModel.MessageTooLong);
        }
        if (
            await _messageRepository.CountMessagesFromAsync(sender, recipient)
            > config.MaxMessagesPerTeam
        )
        {
            return Json(ErrorViewModel.TooManyMessages);
        }

        var message = new Message()
        {
            Sender = sender,
            Recipient = recipient,
            ReadByRecipient = false,
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
