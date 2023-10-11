using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace SocialMedia.Infrastructure.Cache;

public class CacheManager : ICacheManager
{
    private readonly IDistributedCache _cache;

    public CacheManager(IDistributedCache distributedCache)
    {
        _cache = distributedCache;
    }

    public async Task<TResult?> GetAsync<TResult>(string key)
    {
        var dataString = await _cache.GetStringAsync(key);
        return dataString != null ? JsonSerializer.Deserialize<TResult>(dataString) : default;
    }

    public async Task SetAsync<TResult>(string key, TResult data)
    {
        var dataString = JsonSerializer.Serialize(data);
        await _cache.SetStringAsync(key, dataString);
    }

    public async Task RemoveAsync(string key) =>
        await _cache.RemoveAsync(key);

    public async Task RemoveAllAsync(IEnumerable<string> keys)
    {
        foreach (var key in keys)
        {
            await _cache.RemoveAsync(key);
        }
    }
}