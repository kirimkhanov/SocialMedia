using SocialMedia.ApiModels.Posts;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Interfaces;

namespace SocialMedia.Services;

public class PostsService : IPostsService
{
    private readonly IPostRepository _postRepository;
    private readonly IPostsCacheManager _postsCacheManager;
    private readonly IPostCreationHandler _postCreationHandler;

    public PostsService(IPostRepository postRepository, IPostsCacheManager postsCacheManager, IPostCreationHandler postCreationHandler)
    {
        _postRepository = postRepository;
        _postsCacheManager = postsCacheManager;
        _postCreationHandler = postCreationHandler;
    }

    public async Task<Post?> GetPost(int postId) =>
        await _postRepository.GetById(postId);


    public async Task CreatePost(PostDto postDto)
    {
        var post = postDto.ToDbEntry();
        var postId = await _postRepository.AddAsync(post);
        post.Id = postId;

        await _postCreationHandler.Handle(post);
    }

    public async Task UpdatePost(PostDto postDto)
    {
        var post = await _postRepository.GetById(postDto.Id) ??
                   throw new ArgumentException($"Пост с id = {postDto.Id} не найден");

        post.Text = postDto.Text;
        await _postRepository.UpdateAsync(post);

        await _postsCacheManager.UpdatePostInAllFollowersCache(post);
    }

    public async Task DeletePost(int postId)
    {
        var post = await _postRepository.GetById(postId) ??
                   throw new ArgumentException($"Пост с id = {postId} не найден");

        post.IsDeleted = true;
        await _postRepository.UpdateAsync(post);

        await _postsCacheManager.InvalidateAllFollowersCache(post.UserId);
    }
}