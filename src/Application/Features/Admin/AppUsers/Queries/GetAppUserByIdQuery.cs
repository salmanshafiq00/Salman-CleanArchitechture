using System.Text.Json.Serialization;
using CleanArchitechture.Application.Common.Abstractions.Identity;

namespace CleanArchitechture.Application.Features.Admin.AppUsers.Queries;

public record GetAppUserByIdQuery(string Id) 
    : ICacheableQuery<AppUserModel>
{
    [JsonIgnore]
    public string CacheKey => CacheKeys.AppUser;

    public bool? AllowCache => false;

    public TimeSpan? Expiration => null;
}

internal sealed class GetAppUserByIdQueryHandler(IIdentityService identityService) 
    : IQueryHandler<GetAppUserByIdQuery, AppUserModel>
{
    public async Task<Result<AppUserModel>> Handle(GetAppUserByIdQuery request, CancellationToken cancellationToken)
    {
        if(string.IsNullOrEmpty(request.Id) || Guid.Parse(request.Id) == Guid.Empty)
        {
            return new AppUserModel();
        }
        return await identityService.GetUserAsync(request.Id, cancellationToken).ConfigureAwait(false);
    }
}
