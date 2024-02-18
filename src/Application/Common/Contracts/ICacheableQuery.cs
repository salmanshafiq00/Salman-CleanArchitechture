namespace CleanArchitechture.Application.Common.Contracts;

public interface ICacheableQuery<TResponse> : IQuery<TResponse>, ICacheableQuery;

public interface ICacheableQuery
{
    string CacheKey { get; }
    TimeSpan? Expiration { get; }
}


