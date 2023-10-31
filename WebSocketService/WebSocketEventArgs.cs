namespace WebSocketService;

public class WebSocketEventArgs
{
    public string SocketId { get; set; }
    public WebSocketEventArgs(string socketId)
    {
        SocketId = socketId;
    }
}