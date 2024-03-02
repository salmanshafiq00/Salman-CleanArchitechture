using CleanArchitechture.Application.Common.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace CleanArchitechture.Application.Common.Behaviours;

internal sealed class CacheInvalidationBehaviour<TRequest, TResponse>(
    ILogger<CacheInvalidationBehaviour<TRequest, TResponse>> logger,
    IDistributedCacheService distributedCache)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheInvalidatorCommand<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next().ConfigureAwait(false);
        if (!string.IsNullOrEmpty(request.CacheKey))
        {
            await distributedCache.RemoveByPrefixAsync(request.CacheKey, CancellationToken.None);
            logger.LogInformation("Cach value of {CacheKey} expired with {@Request}", request.CacheKey, request);

        }
        
        return response;
    }
}
