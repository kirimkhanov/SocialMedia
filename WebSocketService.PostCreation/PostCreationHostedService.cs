using MessageBrokers.Abstract;

namespace WebSocketService.PostCreation;

public class PostCreationHostedService: IHostedService
{
    private readonly IConsumer _consumer;
    private readonly ILogger<PostCreationHostedService> _logger;

    public PostCreationHostedService(IConsumer consumer, ILogger<PostCreationHostedService> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting service");
        _consumer.BeginConsuming(cancellationToken);
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping service");
        
        return Task.CompletedTask;
    }
}