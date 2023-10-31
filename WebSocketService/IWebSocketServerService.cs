using System.Net.WebSockets;

namespace WebSocketService;

public interface IWebSocketServerService
{
    event EventHandler<WebSocketEventArgs> ClientAdded;
    event EventHandler<WebSocketEventArgs> ClientRemoved;
    Task InitializeSocket(string socketId, WebSocket socket, CancellationToken stoppingToken);

    Task SendMessageToClientAsync(string socketId, string message);
}