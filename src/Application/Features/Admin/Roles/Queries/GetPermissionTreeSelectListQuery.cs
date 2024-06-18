using System.Text.Json.Serialization;
using CleanArchitechture.Application.Common.Abstractions.Identity;

namespace CleanArchitechture.Application.Features.Admin.Roles.Queries;

public record GetPermissionTreeSelectListQuery
    : ICacheableQuery<List<TreeNodeModel>>
{
    [JsonIgnore]
    public string CacheKey => $"Role_Permissions";

    public bool? AllowCache => false;

    public TimeSpan? Expiration => null;
}

internal sealed class GetPermissionNodeListQueryHandler(IIdentityRoleService roleService)
    : IQueryHandler<GetPermissionTreeSelectListQuery, List<TreeNodeModel>>
{
    public async Task<Result<List<TreeNodeModel>>> Handle(GetPermissionTreeSelectListQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(roleService.GetAllPermissions().Value.ToList());
    }
}
