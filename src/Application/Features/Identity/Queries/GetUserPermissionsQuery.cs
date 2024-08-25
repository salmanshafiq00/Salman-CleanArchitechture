using CleanArchitechture.Application.Common.Abstractions.Caching;
using CleanArchitechture.Application.Common.Abstractions.Identity;

namespace CleanArchitechture.Application.Features.Identity.Queries;

public record GetUserPermissionsQuery(string userId) : ICacheableQuery<string[]>
{
    public string CacheKey => $"{CacheKeys.Role}_permissions_{userId}";

    public TimeSpan? Expiration => null;

    public bool? AllowCache => true;
}

internal sealed class GetUserPermissionsQueryHandler(IIdentityService identityService) : IQueryHandler<GetUserPermissionsQuery, string[]>
{
    public async Task<Result<string[]>> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
    {
        return await identityService.GetUserPermissionsAsync(request.userId, cancellationToken);
    }
}
