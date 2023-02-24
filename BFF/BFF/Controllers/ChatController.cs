using System.Diagnostics;
using BFF.Models;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace BFF.Controllers;

[ApiController]
[Route("[controller]")]
public class ChatController : ControllerBase
{
    private readonly ILogger<ChatController> _logger;
    private readonly IAmACommandProcessor _commandProcessor;
    private readonly MessageModel _messageModel;

    public ChatController(ILogger<ChatController> logger, IAmACommandProcessor commandProcessor, MessageModel messageModel )
    {
        _logger = logger;
        _commandProcessor = commandProcessor;
        _messageModel = messageModel;
    }

    [HttpGet]
    public ActionResult<IEnumerable<ChatMessage>> Get()
    {
        return Ok( _messageModel.Get() );
    }

    [HttpPost]
    public void SendMessage( ChatMessage message )
    {
        // https://brightercommand.gitbook.io/paramore-brighter-documentation/brighter-configuration/brighterbasicconfiguration#using-an-external-bus
        _commandProcessor.Publish(new ChatMessageSent( message ));
    }
}

public record class ChatMessageSent(ChatMessage messageToSend) : IEvent {
    public Activity Span { get; set; } = new Activity( nameof(ChatMessageSent) );
    public Guid Id { get; set; } = Guid.NewGuid();
}

public record struct ChatMessage(string SenderUsername, string Message) {}