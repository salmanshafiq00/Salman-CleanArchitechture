using System.Text.Json.Serialization;
using CleanArchitechture.Application.Common.Abstractions.Identity;

namespace CleanArchitechture.Application.Features.Admin.Roles.Queries;

public record GetPermissionsByRoleQuery(string RoleId) 
    : ICacheableQuery<IList<TreeNodeModel<Guid>>>
{
    [JsonIgnore]
    public string CacheKey => $"Role_{RoleId}_Permissions";

    public bool? AllowCache => false;

    public TimeSpan? Expiration => null;
}

internal sealed class GetPermissionsByRoleQueryHandler(IIdentityRoleService roleService) 
    : IQueryHandler<GetPermissionsByRoleQuery, IList<TreeNodeModel<Guid>>>
{
    public async Task<Result<IList<TreeNodeModel<Guid>>>> Handle(GetPermissionsByRoleQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult( roleService.GetAllPermissions(request.RoleId));
    }
}
