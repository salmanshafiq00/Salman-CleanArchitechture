using Microsoft.AspNetCore.Authorization;

namespace WebApi.Infrastructure.Permissions;
public record PermissionRequirement(string Permission) : IAuthorizationRequirement;

