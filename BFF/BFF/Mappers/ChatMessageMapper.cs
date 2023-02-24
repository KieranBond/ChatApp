using BFF.Controllers;
using Paramore.Brighter;
using System.Text;
using System.Text.Json;

namespace BFF.Mappers;

public sealed class ChatMessageMapper : IAmAMessageMapper<ChatMessageSent>
{
    public Message MapToMessage(ChatMessageSent request)
    {
        var header = new MessageHeader(request.Id, nameof(ChatMessageSent), MessageType.MT_EVENT);
        var body = new MessageBody( JsonSerializer.Serialize(request.messageToSend) );
        return new Message( header, body );
    }

    public ChatMessageSent MapToRequest(Message message)
    {
        var chatMessage = JsonSerializer.Deserialize<ChatMessage>(message.Body.Value);
        var evt = new ChatMessageSent(chatMessage);
        evt.Id = message.Id;

        return evt;
    }
}