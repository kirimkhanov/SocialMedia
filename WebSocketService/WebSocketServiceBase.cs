using System.Net.WebSockets;
using System.Text;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace WebSocketService;

public abstract class WebSocketServiceBase
{
    private readonly ILogger _logger;
    public event EventHandler<WebSocketEventArgs>? ClientAdded;
    public event EventHandler<WebSocketEventArgs>? ClientRemoved;

    protected WebSocketServiceBase(ILogger logger, ConnectionManager webSocketConnectionManager)
    {
        _logger = logger;
        WebSocketConnectionManager = webSocketConnectionManager;
    }

    protected ConnectionManager WebSocketConnectionManager { get; set; }

    public async Task InitializeSocket(string socketId, WebSocket socket, CancellationToken stoppingToken)
    {
        try
        {
            await OnConnected(socketId, socket);

            var buffer = new ArraySegment<byte>(new byte[1024 * 4]);

            while (!stoppingToken.IsCancellationRequested)
            {
                // wait for the next ws message to arrive
                var result = await socket.ReceiveAsync(buffer, stoppingToken);

                switch (result.MessageType)
                {
                    case WebSocketMessageType.Close:
                        await OnDisconnected(socket, stoppingToken);
                        break;
                    case WebSocketMessageType.Binary:
                        break;
                    default:
                        _logger.LogDebug("{MessageType}", result.MessageType);
                        break;
                }
            }
        }
        catch (WebSocketException e) when (e.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
        {
            _logger.LogWarning(e, "Connection Lost");
            throw;
        }
        catch (WebSocketException e) when (e.WebSocketErrorCode == WebSocketError.InvalidState)
        {
            _logger.LogWarning(e, "Invalid State");
            throw;
        }
        catch (OperationCanceledException e)
        {
            _logger.LogDebug(e, "Operation Cancelled");
            await WebSocketConnectionManager.RemoveSocket(WebSocketConnectionManager.GetId(socket), stoppingToken);
        }
        catch (Exception e)
        {
            _logger.LogDebug(e, "Error Receiving Message");
            throw;
        }
    }
    
    protected async Task SendMessageAsync(WebSocket webSocket, string message)
    {
        using (LogContext.PushProperty("socket", webSocket, destructureObjects: true))
        using (LogContext.PushProperty("message", message))
        {
            if (webSocket.State != WebSocketState.Open)
            {
                _logger.LogWarning("Socket is not open!");
                return;
            }

            try
            {
                await webSocket.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(message),
                        0,
                        message.Length),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Problem with sending message!");
            }
        }
    }

    private async Task OnConnected(string socketId, WebSocket socket)
    {
        WebSocketConnectionManager.AddSocket(socketId, socket);
        ClientAdded?.Invoke(this, new WebSocketEventArgs(socketId));
        _logger.LogInformation("A connection is established in server with {SocketId}", socketId);
        await Task.CompletedTask;
    }

    private async Task OnDisconnected(WebSocket socket, CancellationToken stoppingToken)
    {
        var socketId = WebSocketConnectionManager.GetId(socket);
        _logger.LogInformation("A connection is closed in server with {SocketId}", socketId);
        await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", stoppingToken);
        await WebSocketConnectionManager.RemoveSocket(socketId, stoppingToken);
        ClientRemoved?.Invoke(this, new WebSocketEventArgs(socketId));
    }
}