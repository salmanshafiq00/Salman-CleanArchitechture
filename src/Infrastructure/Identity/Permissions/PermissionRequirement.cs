using Microsoft.AspNetCore.Authorization;

namespace CleanArchitechture.Infrastructure.Identity.Permissions;
public record PermissionRequirement(string Permission) : IAuthorizationRequirement;

