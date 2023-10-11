using SocialMedia.ApiModels.Posts;
using SocialMedia.Core.Entities;

namespace SocialMedia.Services;

public interface IPostsService
{
    Task<Post?> GetPost(int postId);
    Task CreatePost(PostDto postDto);
    Task UpdatePost(PostDto postDto);
    Task DeletePost(int postId);
}