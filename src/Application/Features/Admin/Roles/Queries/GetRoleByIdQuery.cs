using System.Text.Json.Serialization;
using CleanArchitechture.Application.Common.Abstractions.Identity;

namespace CleanArchitechture.Application.Features.Admin.Roles.Queries;

public record GetRoleByIdQuery(string Id) 
    : ICacheableQuery<RoleModel>
{
    [JsonIgnore]
    public string CacheKey => $"Role_{Id}";

    public bool? AllowCache => false;

    public TimeSpan? Expiration => null;
}

internal sealed class GetRoleByIdQueryHandler(IIdentityRoleService roleService) 
    : IQueryHandler<GetRoleByIdQuery, RoleModel>
{
    public async Task<Result<RoleModel>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        return await roleService.GetRoleAsync(request.Id, cancellationToken).ConfigureAwait(false);
    }
}
