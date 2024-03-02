using CleanArchitechture.Application.Common.Abstractions.Identity;
using Serilog.Context;

namespace CleanArchitechture.Application.Common.Behaviours;

internal sealed class RequestLoggingBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : Result
{
    private readonly ILogger _logger;
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public RequestLoggingBehaviour(ILogger<TRequest> logger, IUser user, IIdentityService identityService)
    {
        _logger = logger;
        _user = user;
        _identityService = identityService;

    }
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _user.Id ?? string.Empty;
        string? userName = string.Empty;

        if (!string.IsNullOrEmpty(userId))
        {
            userName = await _identityService.GetUserNameAsync(userId);
        }

        _logger.LogInformation("Processing Request: {Name} {@UserId} {@UserName} {@Request}",
            requestName, userId, userName, request);

        TResponse result = await next().ConfigureAwait(false);

        if(result.IsSuccess)
        {
            _logger.LogInformation("Completed Request: {@RequestName}, {@DateTimeUtc}", requestName, DateTime.Now);
        }
        else
        {
            using (LogContext.PushProperty("Error", result.Error, true))
            {
                _logger.LogError("Completed Request with Failure: {RequestName}", requestName);
            }
        }

        return result;
    }
}
