using CleanArchitechture.Application.Common.Abstractions.Caching;
using CleanArchitechture.Application.Common.Abstractions.Identity;

namespace CleanArchitechture.Application.Features.Admin.AppUsers.Queries;

public record GetAppUserProfileQuery(string userId) 
    : ICacheableQuery<AppUserModel>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.AppUser}_{userId}";

    public bool? AllowCache => true;

    public TimeSpan? Expiration => null;
}

internal sealed class GetAppUserProfileQueryHandler(IIdentityService identityService) 
    : IQueryHandler<GetAppUserProfileQuery, AppUserModel>
{
    public async Task<Result<AppUserModel>> Handle(GetAppUserProfileQuery request, CancellationToken cancellationToken)
    {
        if(string.IsNullOrEmpty(request.userId) || Guid.Parse(request.userId) == Guid.Empty)
        {
            return new AppUserModel();
        }
        return await identityService.GetUserAsync(request.userId, cancellationToken).ConfigureAwait(false);
    }
}
