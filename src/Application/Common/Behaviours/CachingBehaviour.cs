using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace CleanArchitechture.Application.Common.Behaviours;


#region Inmemory Cache

//internal sealed class CachingBehaviour<TRequest, TResponse>(
//    IInMemoryCacheService cacheService)
//    : IPipelineBehavior<TRequest, TResponse>
//    where TRequest : ICachedQuery
//{
//    public async Task<TResponse> Handle(
//        TRequest request,
//        RequestHandlerDelegate<TResponse> next,
//        CancellationToken cancellationToken)
//    {
//        return await cacheService.GetOrCreateAsync(
//            request.CacheKey,
//            _ => next(),
//            request.Expiration,
//            cancellationToken);
//    }
//}

#endregion

#region using distributed cache

internal sealed class CachingBehaviour<TRequest, TResponse>(
    IDistributedCacheService cacheService,
    ILogger<CachingBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableQuery
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {

        var cachedResponse = await cacheService
            .GetStringAsync(request.CacheKey, cancellationToken);

        if (cachedResponse is not null)
        {
            logger.LogInformation($"Cache hit for {typeof(TRequest).FullName}");
            return JsonSerializer.Deserialize<TResponse>(cachedResponse);
        }

        // If not found in cache, proceed with the actual request handling
        var response = await next();

        await cacheService.SetStringAsync(
                    request.CacheKey,
                    JsonSerializer.Serialize(response),
                    request.Expiration,
                    cancellationToken);

        return response;
    }
}


#endregion
