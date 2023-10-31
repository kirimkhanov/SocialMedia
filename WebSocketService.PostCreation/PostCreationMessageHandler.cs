using System.Text.Json;
using MessageBrokers;
using MessageBrokers.Abstract;
using MessageBrokers.Payloads;

namespace WebSocketService.PostCreation;

public class PostCreationMessageHandler: IBrokerMessageHandler<PostCreationPayload>
{
    private readonly ILogger<PostCreationMessageHandler> _logger;
    private readonly IWebSocketServerService _webSocketServerService;

    public PostCreationMessageHandler(
        ILogger<PostCreationMessageHandler> logger,
        IWebSocketServerService webSocketServerService)
    {
        _webSocketServerService = webSocketServerService;
        _logger = logger;
    }

    public async Task Handle(Message<PostCreationPayload>  postCreationMessage)
    {
        _logger.LogInformation($"Received message with payload: {JsonSerializer.Serialize(postCreationMessage.Payload)}");

        var webSocketMessage = new 
        {
            postId = postCreationMessage.Payload.PostId,
            postText = postCreationMessage.Payload.PostText,
            userId = postCreationMessage.Payload.UserId
        };
        await _webSocketServerService.SendMessageToClientAsync(postCreationMessage.Payload.SocketId, JsonSerializer.Serialize(webSocketMessage));
        _logger.LogInformation($"Sent message to client via websocket. SocketId: {postCreationMessage.Payload.SocketId}");
    }
}