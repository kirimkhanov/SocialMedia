using MessageBrokers.Abstract;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace MessageBrokers.RabbitMq;

public class RabbitMqService: IRabbitMqService
{
    private readonly RabbitMqConfiguration _configuration;
    public RabbitMqService(IOptions<RabbitMqConfiguration> options)
    {
        _configuration = options.Value;
    }
    public IConnection CreateChannel()
    {
        var connection = new ConnectionFactory
        {
            UserName = _configuration.UserName,
            Password = _configuration.Password,
            HostName = _configuration.Hostname,
            DispatchConsumersAsync = true
        };
        var channel = connection.CreateConnection();
        return channel;
    }
}