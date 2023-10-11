using SocialMedia.Core.Entities;

namespace SocialMedia.Core.Interfaces;

public interface IFollowRepository
{
    Task<IEnumerable<Follow>> GetFollows(int userId);
    Task<Follow> GetByFolloweeIdAndFollowerId(int followeeId, int followerId);
    Task AddAsync(Follow follow);
    Task RemoveAsync(Follow follow);
}