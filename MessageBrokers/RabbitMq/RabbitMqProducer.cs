using System.Text;
using System.Text.Json;
using MessageBrokers.Abstract;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace MessageBrokers.RabbitMq
{
    public class RabbitMqProducer : IProducer, IDisposable
    {
        private readonly IModel _model;
        private readonly IConnection _connection;
        private readonly IBasicProperties _messageProperties;
        private readonly ILogger<RabbitMqProducer> _logger;
        private readonly RabbitMqConfiguration _rabbitMqConfiguration;

        public RabbitMqProducer(ILogger<RabbitMqProducer> logger, IOptions<RabbitMqConfiguration> rabbitMqOptions, IRabbitMqService rabbitMqService)
        {
            _rabbitMqConfiguration = rabbitMqOptions.Value;
            _logger = logger;
            
            
            _logger.LogInformation("RabbitMq publisher initialization");

            _connection = rabbitMqService.CreateChannel();
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            _model = _connection.CreateModel();
            _model.QueueDeclare(queue: _rabbitMqConfiguration.QueueName, durable: true, exclusive: false,
                autoDelete: false, arguments: null);
            _model.ConfirmSelect();
            _messageProperties = _model.CreateBasicProperties();
            _messageProperties.DeliveryMode = _rabbitMqConfiguration.DeliveryMode;
            _messageProperties.Persistent = true;
        }

        public Task<object> Send<TPayloadType>(string routingKey, Message<TPayloadType> message)
            where TPayloadType : IPayload
        {
            try
            {
                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                _model.BasicPublish(_rabbitMqConfiguration.Exchange ?? "", routingKey, _messageProperties, body);
                
                _model.WaitForConfirmsOrDie();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send message with id {message.MessageId}.", ex.Message);
                throw;
            }

            return Task.FromResult<object>(null);
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogInformation("RabbitMQ connection is closed");
        }

        public void Dispose()
        {
            _model?.Dispose();
            _connection?.Dispose();
        }
    }
}