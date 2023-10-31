using SocialMedia.Core.Entities;
using SocialMedia.Core.Entities.Users;

namespace SocialMedia.Services;

public interface IPostsCacheManager
{
    Task<IEnumerable<Post>> GetPosts(PostSearchParams searchParams);
    Task AddPostToAllFollowersCache(Post post);
    Task UpdatePostInAllFollowersCache(Post post);
    Task InvalidateAllFollowersCache(int userId);
    Task AddPostToUserCache(int userId, Post post);
}