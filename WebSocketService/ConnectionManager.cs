using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace WebSocketService;

public class ConnectionManager
{
    private readonly ConcurrentDictionary<string, WebSocket> _sockets = new();

    public ConcurrentDictionary<string, WebSocket> GetAll()
    {
        return _sockets;
    }

    public async IAsyncEnumerable<KeyValuePair<string, WebSocket>> GetAllAsync()
    {
        foreach (var webSocket in _sockets)
        {
            yield return webSocket;
        }

        await Task.CompletedTask;
    }

    public WebSocket GetBySocketId(string socketId)
    {
        return _sockets.FirstOrDefault(p => p.Key == socketId).Value;
    }

    public string GetId(WebSocket socket)
    {
        return _sockets.FirstOrDefault(p => p.Value == socket).Key;
    }

    public void AddSocket(string socketId, WebSocket socket)
    {
        _sockets.TryAdd(socketId, socket);
    }

    public async Task RemoveSocket(string socketId, CancellationToken stoppingToken)
    {
        _sockets.TryRemove(socketId, out var socket);

        if (socket != null && (socket.State == WebSocketState.CloseReceived || socket.State == WebSocketState.CloseSent || socket.State == WebSocketState.Open))
        {
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the ConnectionManager", stoppingToken);
        }
    }
}