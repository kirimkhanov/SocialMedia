using SocialMedia.Core.Entities;
using SocialMedia.Core.Entities.Users;

namespace SocialMedia.Core.Interfaces;

public interface IPostRepository
{
    Task<Post?> GetById(int postId);
    Task<IEnumerable<Post>> GetPosts(PostSearchParams postSearchParams);
    Task<int> AddAsync(Post post);
    Task UpdateAsync(Post post);
}