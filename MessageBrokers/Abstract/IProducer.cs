namespace MessageBrokers.Abstract
{
    public interface IProducer
    {
        Task<object> Send<TPayloadType>(string routingKey, Message<TPayloadType> message)
            where TPayloadType : IPayload;
    }
}