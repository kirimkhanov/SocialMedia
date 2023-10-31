namespace MessageBrokers.Abstract
{
    public interface IBrokerMessageHandler<TPayload> where TPayload : IPayload
    {
        Task Handle(Message<TPayload> message);
    }
}