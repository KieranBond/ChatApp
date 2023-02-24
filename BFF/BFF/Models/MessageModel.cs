using BFF.Controllers;

namespace BFF.Models;

public sealed record class MessageModel 
{
    private readonly List<ChatMessage> _messages = new();

    public void Save(ChatMessage message) => _messages.Add(message);

    public IReadOnlyList<ChatMessage> Get() => _messages.AsReadOnly();
}