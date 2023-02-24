using BFF.Controllers;
using BFF.Models;
using Paramore.Brighter;

namespace BFF.Handlers;

public class ChatMessageHandler : RequestHandler<ChatMessageSent> 
{
    private readonly MessageModel _model;

    public ChatMessageHandler( MessageModel model ) 
    {
        _model = model;
    }

    public override ChatMessageSent Handle(ChatMessageSent command)
    {
        _model.Save( command.messageToSend );
        return base.Handle(command);
    }
}