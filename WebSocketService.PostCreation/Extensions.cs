using System.Security.Claims;

namespace WebSocketService.PostCreation;

public static class Extensions
{
    public static string GetMachineName()
    {
        var machineName = Environment.MachineName;
        if (string.IsNullOrWhiteSpace(machineName))
            machineName = Environment.GetEnvironmentVariable("COMPUTERNAME");

        if (string.IsNullOrWhiteSpace(machineName))
            machineName = Environment.GetEnvironmentVariable("HOSTNAME");

        if (string.IsNullOrWhiteSpace(machineName))
            machineName =
                $"{Environment.GetEnvironmentVariable("CF_INSTANCE_INDEX")};{Environment.GetEnvironmentVariable("CF_INSTANCE_ADDR")}";

        return machineName;
    }

    private static void AddWebSocketManager(this IServiceCollection services)
    {
        services.AddSingleton<ConnectionManager>();
        services.AddSingleton<WebSocketServerService>();
        services.AddSingleton<IWebSocketServerService, WebSocketServerService>();
    }

    private static IApplicationBuilder PrivateUseWebSocket(this IApplicationBuilder app,
        WebSocketOptions options = null)
    {
        if (options == null)
            app.UseWebSockets();
        else
            app.UseWebSockets(options);

        return app;
    }

    public static IApplicationBuilder UseWebSocketServer(this IApplicationBuilder app, PathString path,
        WebSocketOptions options = null)
    {
        app.PrivateUseWebSocket(options);
        return app.Map(path, builder => builder.UseMiddleware<WebSocketServerMiddleware>());
    }

    public static IServiceCollection AddWebSocketServer(this IServiceCollection services)
    {
        services.AddWebSocketManager();

        return services;
    }
    
    public static int? GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        var claim = claimsPrincipal.Claims.FirstOrDefault(c =>
            c.Type == ClaimsIdentity.DefaultNameClaimType);
        if (int.TryParse(claim?.Value, out var userId))
            return userId;

        return null;
    }
}