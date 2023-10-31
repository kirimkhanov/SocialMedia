using System.Text;
using System.Text.Json;
using MessageBrokers.Abstract;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MessageBrokers.RabbitMq
{
    public class RabbitMqConsumer<TPayloadType> : IConsumer, IDisposable
        where TPayloadType : IPayload
    {
        private readonly IModel _model;
        private readonly IConnection _connection;
        private readonly IBrokerMessageHandler<TPayloadType> _handler;
        private readonly ILogger<RabbitMqConsumer<TPayloadType>> _logger;
        private readonly RabbitMqConfiguration _rabbitMqConfiguration;


        public RabbitMqConsumer(
            IBrokerMessageHandler<TPayloadType> handler,
            ILogger<RabbitMqConsumer<TPayloadType>> logger,
            IOptions<RabbitMqConfiguration> rabbitMqOptions, IRabbitMqService rabbitMqService)
        {
            _rabbitMqConfiguration = rabbitMqOptions.Value;
            _handler = handler;
            _logger = logger;

            _logger.LogInformation("RabbitMq consumer initialization");

            _connection = rabbitMqService.CreateChannel();
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            _model = _connection.CreateModel();
            _model.QueueDeclare(queue: _rabbitMqConfiguration.QueueName, durable: true, exclusive: false,
                autoDelete: false, arguments: null);
        }

        public void BeginConsuming(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(_model);
            consumer.Received += OnConsumerReceived;
            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerCancelled;

            _model.BasicConsume(_rabbitMqConfiguration.QueueName, false, consumer);
        }

        private async Task OnConsumerReceived(object sender, BasicDeliverEventArgs e)
        {
            try
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                var obj = JsonSerializer.Deserialize<Message<TPayloadType>>(message);
                await _handler.Handle(obj);
                _model.BasicAck(e.DeliveryTag, false);
            }
            catch (JsonException)
            {
                _logger.LogWarning("Json deserialization failed while handling a message.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private Task OnConsumerCancelled(object sender, ConsumerEventArgs e)
        {
            _logger.LogWarning("Consume operation was cancelled.");
            return Task.CompletedTask;
        }

        private Task OnConsumerUnregistered(object sender, ConsumerEventArgs e)
        {
            _logger.LogInformation("Consumer is unregistered");
            return Task.CompletedTask;
        }

        private Task OnConsumerRegistered(object sender, ConsumerEventArgs e)
        {
            _logger.LogInformation("Consumer is registered");
            return Task.CompletedTask;
        }

        private Task OnConsumerShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogInformation("Consumer is closed");
            return Task.CompletedTask;
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