using MessageBrokers.Abstract;

namespace MessageBrokers.Payloads;

public class PostCreationPayload : IPayload
{
    public int PostId { get; set; }
    public string PostText { get; set; }
    public int UserId { get; set; }
    public string SocketId { get; set; }
}