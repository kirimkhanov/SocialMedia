using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SocialMedia;
using SocialMedia.Core.Interfaces;
using SocialMedia.Core.Services;
using SocialMedia.Infrastructure.Cache;
using SocialMedia.Infrastructure.Data.Repositories;
using SocialMedia.Middlewares;
using SocialMedia.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.OperationFilter<SwaggerJsonIgnoreFilter>();
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});
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

var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
var slaveConnection = builder.Configuration.GetConnectionString("SlaveConnection");
var citusConnection = builder.Configuration.GetConnectionString("citusConnection");
builder.Services.AddTransient<IUserRepository, UserRepository>(ur =>
    new UserRepository(defaultConnection, slaveConnection));
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IAuthService, AuthService>();

builder.Services.AddTransient<IPostsService, PostsService>();
builder.Services.AddTransient<IFollowsService, FollowsService>();
builder.Services.AddTransient<IDialogService, DialogService>();

builder.Services.AddTransient<ICacheManager, CacheManager>();
builder.Services.AddTransient<IPostsCacheManager, PostsCacheManager>();

builder.Services.AddTransient<IFollowRepository, FollowRepository>(ur =>
    new FollowRepository(defaultConnection));
builder.Services.AddTransient<IPostRepository, PostRepository>(ur =>
    new PostRepository(defaultConnection));
builder.Services.AddTransient<IDialogMessageRepository, DialogMessageRepository>(ur =>
    new DialogMessageRepository(citusConnection));

builder.Services.AddHttpContextAccessor();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost";
    options.InstanceName = "local";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandling();

app.Run();