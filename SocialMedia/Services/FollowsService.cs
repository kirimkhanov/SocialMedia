using SocialMedia.Core.Entities;
using SocialMedia.Core.Interfaces;

namespace SocialMedia.Services;

public class FollowsService : IFollowsService
{
    private readonly IFollowRepository _followRepository;

    public FollowsService(IFollowRepository followRepository)
    {
        _followRepository = followRepository;
    }

    public async Task AddFollower(int followerId, int followeeId)
    {
        var follow = new Follow
        {
            FolloweeId = followeeId,
            FollowerId = followerId
        };

        await _followRepository.AddAsync(follow);
    }

    public async Task DeleteFollower(int followerId, int followeeId)
    {
        var follow = await _followRepository.GetByFolloweeIdAndFollowerId(followeeId, followerId);
        follow.IsDeleted = true;
        await _followRepository.RemoveAsync(follow);
    }
}