using MessageBrokers;
using MessageBrokers.Abstract;
using MessageBrokers.Payloads;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Interfaces;
using SocialMedia.Infrastructure.Cache;

namespace SocialMedia.Services;

public class PostCreationHandler: IPostCreationHandler
{
    private readonly IPostsCacheManager _postsCacheManager;
    private readonly IProducer _producer;
    private readonly IFollowRepository _followRepository;
    private readonly ICacheManager _cacheManager;

    public PostCreationHandler(IPostsCacheManager postsCacheManager, IProducer producer, IFollowRepository followRepository, ICacheManager cacheManager)
    {
        _postsCacheManager = postsCacheManager;
        _producer = producer;
        _followRepository = followRepository;
        _cacheManager = cacheManager;
    }

    public async Task Handle(Post post)
    {
        var message = new Message<PostCreationPayload>()
        {
            MessageId = Guid.NewGuid().ToString(),
            Payload = new PostCreationPayload()
            {
                PostId = post.Id,
                PostText = post.Text,
                UserId = post.UserId,
            }
        };
        var follows = await _followRepository.GetFollows(post.UserId);
        foreach (var follow in follows)
        {
            await _postsCacheManager.AddPostToUserCache(follow.FollowerId, post);
            var queueName = await _cacheManager.GetAsync<string?>(GetKeyString(follow.FollowerId));
            if(queueName is null) continue;

            message.Payload.SocketId = follow.FollowerId.ToString();
            await _producer.Send(queueName, message);
        }
    }
    
    private string GetKeyString(int userId) => $"websocket_client_{userId.ToString()}";
}