using System.Text.Json.Serialization;
using CleanArchitechture.Application.Common.Abstractions.Identity;

namespace CleanArchitechture.Application.Features.Admin.AppUsers.Queries;

public record GetAppUserByIdQuery(string id) 
    : ICacheableQuery<AppUserModel>
{
    [JsonIgnore]
    public string CacheKey => CacheKeys.AppUser;

    public bool? AllowCache => true;

    public TimeSpan? Expiration => null;
}

internal sealed class GetAppUserByIdQueryHandler(IIdentityService identityService) 
    : IQueryHandler<GetAppUserByIdQuery, AppUserModel>
{
    public async Task<Result<AppUserModel>> Handle(GetAppUserByIdQuery request, CancellationToken cancellationToken)
    {
        return await identityService.GetUserAsync(request.id, cancellationToken).ConfigureAwait(false);
    }
}
