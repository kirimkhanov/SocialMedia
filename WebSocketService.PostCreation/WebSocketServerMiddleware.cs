using System.Net.WebSockets;
using MessageBrokers.RabbitMq;
using Microsoft.Extensions.Options;
using SocialMedia.Infrastructure.Cache;

namespace WebSocketService.PostCreation;

public class WebSocketServerMiddleware
{
    private readonly IWebSocketServerService _webSocketServerService;
    private readonly ILogger<WebSocketServerMiddleware> _logger;
    private readonly RabbitMqConfiguration _rabbitMqConfiguration;
    private readonly ICacheManager _cacheManager;

    public WebSocketServerMiddleware(RequestDelegate next, IWebSocketServerService webSocketServerService,
        ILogger<WebSocketServerMiddleware> logger, IOptions<RabbitMqConfiguration> rabbitMqConfiguration,
        ICacheManager cacheManager)
    {
        _webSocketServerService = webSocketServerService;
        _logger = logger;
        _rabbitMqConfiguration = rabbitMqConfiguration.Value;
        _cacheManager = cacheManager;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stoppingToken = context.RequestAborted;

        if (context.WebSockets.IsWebSocketRequest)
        {
            // create a websocket as a listener 
            var socket = await context.WebSockets.AcceptWebSocketAsync();
            var userId = context.User.GetUserId() ?? throw new Exception("Client is not authorized");
            try
            {
                _webSocketServerService.ClientAdded += OnClientAdded;
                _webSocketServerService.ClientRemoved += OnClientRemoved;
                await _webSocketServerService.InitializeSocket(userId.ToString(), socket, stoppingToken);
            }
            catch (WebSocketException)
            {
                _logger.LogWarning("Connection Lost in the middleware");
                throw;
            }
        }
    }

    private void OnClientAdded(object sender, WebSocketEventArgs e)
    {
        _cacheManager.SetAsync(GetKeyString(e.SocketId), _rabbitMqConfiguration.QueueName);
        _logger.LogInformation("Client is added to cache");
    }

    private void OnClientRemoved(object sender, WebSocketEventArgs e)
    {
        _cacheManager.RemoveAsync(GetKeyString(e.SocketId));
        _logger.LogInformation("Client is removed from cache");
    }
    
    private string GetKeyString(string socketId) => $"websocket_client_{socketId}";
}