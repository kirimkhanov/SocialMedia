namespace SocialMedia.Infrastructure.Cache;

public interface ICacheManager
{
    Task<TResult?> GetAsync<TResult>(string key);
    Task SetAsync<TResult>(string key, TResult data);
    Task RemoveAsync(string key);
    Task RemoveAllAsync(IEnumerable<string> keys);
}