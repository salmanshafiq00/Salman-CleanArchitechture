namespace CleanArchitechture.Application.Common.Abstractions.Messaging;

public interface ICacheInvalidatorCommand<TResponse> : ICommand<TResponse>
{
    string CacheKey => string.Empty;
}

public interface ICacheInvalidatorCommand : ICommand
{
    string CacheKey => string.Empty;
}
