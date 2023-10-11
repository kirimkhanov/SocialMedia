using SocialMedia.Core.Entities;
using SocialMedia.Core.Entities.Users;
using SocialMedia.Core.Interfaces;
using SocialMedia.Infrastructure.Cache;

namespace SocialMedia.Services;

public class PostsCacheManager: IPostsCacheManager
{
    private readonly ICacheManager _cacheManager;
    private readonly IPostRepository _postRepository;
    private readonly IFollowRepository _followRepository;
    private readonly ILogger<PostsCacheManager> _logger;

    public PostsCacheManager(ICacheManager cacheManager, IPostRepository postRepository, 
        IFollowRepository followRepository, ILogger<PostsCacheManager> logger)
    {
        _cacheManager = cacheManager;
        _postRepository = postRepository;
        _followRepository = followRepository;
        _logger = logger;
    }
    
    public async Task<IEnumerable<Post>> GetPosts(PostSearchParams searchParams)
    {
        var keyString = searchParams.UserId.ToString();
        LogDataExtractionFromCache(keyString);
        var postsFromCache = await _cacheManager.GetAsync<IList<Post>>(keyString) ?? Array.Empty<Post>();
        LogDataExtractedFromCache(postsFromCache);
        if (postsFromCache.Any()) return postsFromCache.Skip(searchParams.Offset).Take(searchParams.Limit);

        LogDataExtractionFromDb(keyString);
        var postsFromDb = (await _postRepository.GetPosts(new PostSearchParams
        {
            UserId = searchParams.UserId,
            Offset = 0,
            Limit = 1000
        })).ToList();
        LogDataExtractedFromDb(postsFromDb);
        if (!postsFromDb.Any()) return postsFromDb;

        LogSavingExtractedDataFromDbToCache();
        await _cacheManager.SetAsync(keyString, postsFromDb);
        return postsFromDb.Skip(searchParams.Offset).Take(searchParams.Limit);
    }

    public async Task AddPostToAllFollowersCache(Post post)
    {
        var follows = await _followRepository.GetFollows(post.UserId);

        foreach (var follow in follows)
        {
            await AddPostToUserCache(follow.FollowerId, post);
        }
    }

    public async Task UpdatePostInAllFollowersCache(Post post)
    {
        var follows = await _followRepository.GetFollows(post.UserId);

        foreach (var follow in follows)
        {
            await UpdatePostInUserCache(follow.FollowerId, post);
        }
    }

    public async Task InvalidateAllFollowersCache(int userId)
    {
        var follows = await _followRepository.GetFollows(userId);
        await _cacheManager.RemoveAllAsync(follows.Select(f=>f.FollowerId.ToString()).ToArray());
    }
    
    private async Task AddPostToUserCache(int userId, Post post)
    {
        var keyString = userId.ToString();
        var postsFromCache = await _cacheManager.GetAsync<IList<Post>>(keyString);
        if (postsFromCache is null)
            return;

        postsFromCache.Insert(0, post);
        await _cacheManager.SetAsync(keyString, postsFromCache.Take(1000));
    }
    
    private async Task UpdatePostInUserCache(int userId, Post post)
    {
        var keyString = userId.ToString();
        var postsFromCache = await _cacheManager.GetAsync<IList<Post>>(keyString);
        var postFromCache = postsFromCache?.SingleOrDefault(p => p.Id == post.Id);
        if (postsFromCache is null || postFromCache is null)
            return;

        postFromCache.Text = post.Text;
        await _cacheManager.SetAsync(keyString, postsFromCache);
    }

    private void LogDataExtractionFromCache(string keyString) =>
        _logger.LogDebug($"Извлечение постов из кэша.\n" +
                         $"Параметры поиска: {keyString}\n");

    private void LogDataExtractedFromCache(ICollection<Post> posts) =>
        _logger.LogDebug($"Количество извлеченных постов из кэша: {posts.Count}\n");

    private void LogDataExtractionFromDb(string keyString) =>
        _logger.LogDebug("Извлечение постов из базы данных.\n" +
                         $"Параметры поиска: {keyString}\n");

    private void LogDataExtractedFromDb(ICollection<Post> posts) =>
        _logger.LogDebug($"Количество извлеченных постов из базы данных: {posts.Count}\n");

    private void LogSavingExtractedDataFromDbToCache() =>
        _logger.LogDebug("Запись извлеченных постов из базы данных в кэш");
}