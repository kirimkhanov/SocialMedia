using MessageBrokers.Abstract;
using MessageBrokers.Payloads;
using MessageBrokers.RabbitMq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SocialMedia;
using SocialMedia.Infrastructure.Cache;
using WebSocketService.PostCreation;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection("RabbitMq"));

builder.Services.AddTransient<ICacheManager, CacheManager>();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost";
    options.InstanceName = "local";
});

builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();
builder.Services.AddSingleton<IConsumer, RabbitMqConsumer<PostCreationPayload>>();
builder.Services.AddSingleton<IBrokerMessageHandler<PostCreationPayload>, PostCreationMessageHandler>();
builder.Services.AddHostedService<PostCreationHostedService>();

builder.Services.AddWebSocketServer();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,

            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,
            ValidateLifetime = true,

            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true,
        };
    });

var app = builder.Build();

app.UseAuthentication();

app.UseWebSocketServer("/post/feed/posted");

app.Run();