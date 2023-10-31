namespace MessageBrokers.Abstract
{
    public interface IConsumer
    {
        void BeginConsuming(CancellationToken token);
    }
}