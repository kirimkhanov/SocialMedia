using System.Net.WebSockets;
using Microsoft.Extensions.Logging;

namespace WebSocketService;

public class WebSocketServerService : WebSocketServiceBase, IWebSocketServerService
{
    public WebSocketServerService(ConnectionManager webSocketConnectionManager, ILogger<WebSocketServiceBase> logger) :
        base(logger, webSocketConnectionManager)
    {
    }

    public async Task SendMessageToClientAsync(string socketId, string message)
    {
        var socket = WebSocketConnectionManager.GetBySocketId(socketId);
        if (socket.State == WebSocketState.Open)
            await SendMessageAsync(socket, message);
    }
}