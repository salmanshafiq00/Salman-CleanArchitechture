namespace CleanArchitechture.Application.Common.Contracts;

public interface ICacheInvalidatorCommand<TResponse> : ICommand<TResponse>
{
    string CacheKey => string.Empty;
}
