using BFF.Controllers;
using HotChocolate.Subscriptions;

namespace BFF.Models;

public sealed record class MessageModel 
{
    private readonly ITopicEventSender _eventSender;
    private readonly List<ChatMessage> _messages = new();

    public MessageModel([Service] ITopicEventSender eventSender)
    {
        _eventSender = eventSender;
    }

    public async Task Save(ChatMessage message)
    {
        _messages.Add(message);
        await _eventSender.SendAsync(nameof(ChatMessage), message);
    } 

    public IReadOnlyList<ChatMessage> Get() => _messages.AsReadOnly();
}