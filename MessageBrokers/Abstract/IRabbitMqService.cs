using RabbitMQ.Client;

namespace MessageBrokers.Abstract;

public interface IRabbitMqService
{
    IConnection CreateChannel();
}