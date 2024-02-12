using System.Collections.Concurrent;
using System.Text.Json;
using CleanArchitechture.Application.Common.Caching;
using CleanArchitechture.Infrastructure.OptionsSetup.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace CleanArchitechture.Infrastructure.Caching;

internal sealed class DistributedCacheService(
    IDistributedCache distributedCache,
    IOptions<CacheOptions> cacheOptions)
    : IDistributedCacheService
{
    private readonly CacheOptions _cacheOptions = cacheOptions.Value;
    private static readonly ConcurrentDictionary<string, bool> CacheKeys = new();

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellation = default)
        where T : class
    {
        string? cachedValue = await distributedCache
            .GetStringAsync(key, cancellation);

        return cachedValue is null
            ? null
            : JsonSerializer.Deserialize<T>(cachedValue);
    }

    public async Task<string?> GetStringAsync(string key, CancellationToken cancellation = default)
    {
        return await distributedCache
            .GetStringAsync(key, cancellation);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null, CancellationToken cancellation = default)
        where T : class
    {
        string cacheValue = JsonSerializer.Serialize(value);

        await distributedCache.SetStringAsync(key, cacheValue, GetOptions(slidingExpiration), cancellation);

        CacheKeys.TryAdd(key, false);
    }

    public async Task SetStringAsync(string key, string cacheValue, TimeSpan? slidingExpiration = null , CancellationToken cancellation = default)
    {
        await distributedCache.SetStringAsync(key, cacheValue, GetOptions(slidingExpiration), cancellation);

        CacheKeys.TryAdd(key, false);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellation = default)
    {
        await distributedCache.RemoveAsync(key, cancellation);

        CacheKeys.TryRemove(key, out bool _);
    }

    public async Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellation = default)
    {
        var keys = CacheKeys
            .Keys
            .Where(k => string.Equals(prefixKey, PrefixValue(k)));

        var tasks = CacheKeys
            .Keys
            .Where(k => string.Equals(prefixKey, PrefixValue(k)))
            .Select(k => RemoveAsync(k, cancellation));

        //var tasks = CacheKeys
        //    .Keys
        //    .Where(k => k.StartsWith(prefixKey))
        //    .Select(k => RemoveAsync(k, cancellation));

        await Task.WhenAll(tasks);
    }

    public async Task<T?> GetOrSetAsync<T>(string key,
        Func<Task<T>> factory,
        TimeSpan? slidingExpiration = null,
        CancellationToken cancellation = default)
        where T : class
    {
        T? cachedValue = await GetAsync<T>(key, cancellation);

        if (cachedValue is not null)
            return cachedValue;

        cachedValue = await factory();

        await SetAsync(key, cachedValue, slidingExpiration, cancellation);

        return cachedValue;
    }

    private  DistributedCacheEntryOptions GetOptions(TimeSpan? slidingExpiration)
    {
        var options = new DistributedCacheEntryOptions();
        
        return slidingExpiration.HasValue 
            ? options.SetSlidingExpiration(slidingExpiration.Value)
            : options.SetSlidingExpiration(TimeSpan.FromMinutes(_cacheOptions.SlidingExpiration));
    }

    private static string PrefixValue(string input, char delimiter = '_')
    {
        string[] parts = input.Split(delimiter);
        return parts.Length > 0 ? parts[0] : input;
    }

}
