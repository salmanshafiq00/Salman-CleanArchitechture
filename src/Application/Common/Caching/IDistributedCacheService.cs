
namespace CleanArchitechture.Application.Common.Caching;

public interface IDistributedCacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellation = default)
    where T : class;

    Task<string?> GetStringAsync(string key, CancellationToken cancellation = default);

    Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null, CancellationToken cancellation = default)
        where T : class;

    Task SetStringAsync(string key, string value, TimeSpan? slidingExpiration = null, CancellationToken cancellation = default);

    Task RemoveAsync(string key, CancellationToken cancellation = default);

    Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellation = default);

    Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? slidingExpiration = null , CancellationToken cancellation = default)
        where T : class;

}
