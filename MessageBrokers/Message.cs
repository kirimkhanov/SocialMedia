
using MessageBrokers.Abstract;

namespace MessageBrokers
{
    public class Message<TPayload> where TPayload: IPayload
    {
        public string MessageId { get; set; }
        public TPayload Payload { get; set; }
    }
}