using System.ComponentModel;
using Application.Constants;
using CleanArchitechture.Application.Common.Abstractions.Identity;
using CleanArchitechture.Application.Common.Models;
using CleanArchitechture.Application.Features.Admin.Roles.Commands;
using CleanArchitechture.Application.Features.Admin.Roles.Queries;
using CleanArchitechture.Infrastructure.Identity.Permissions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CleanArchitechture.Application.Common.Security;

namespace CleanArchitechture.Infrastructure.Identity.Services;

internal class IdentityRoleService(
    RoleManager<IdentityRole> roleManager,
    IdentityContext identityContext,
    ILogger<IdentityRoleService> logger)
    : IIdentityRoleService
{
    public async Task<Result<string>> CreateRoleAsync(
        string name,
        CancellationToken cancellation = default)
    {
        var role = new IdentityRole
        {
            Name = name,
            NormalizedName = name.ToUpper()
        };

        var result = await roleManager.CreateAsync(role);

        return result.Succeeded
            ? Result.Success(role.Id)
            : Result.Failure<string>(Error.Failure("Role.Create", ErrorMessages.UNABLE_CREATE_ROLE));
    }

    public async Task<Result> DeleteRoleAsync(
        string id,
        CancellationToken cancellation = default)
    {
        var role = await roleManager.FindByIdAsync(id);

        if (role is null)
            Result.Failure(Error.Failure("Role.Delete", ErrorMessages.ROLE_NOT_FOUND));

        var result = await roleManager.DeleteAsync(role);

        return result.Succeeded
            ? Result.Success()
            : Result.Failure(Error.Failure("Role.Delete", ErrorMessages.UNABLE_DELETE_ROLE));

    }

    public async Task<Result<RoleModel>> GetRoleAsync(
        string id,
        CancellationToken cancellation = default)
    {
        var role = await roleManager.FindByIdAsync(id);

        if (role is null)
            Result.Failure<RoleModel>(Error.Failure("Role.Delete", ErrorMessages.ROLE_NOT_FOUND));

        return Result.Success(new RoleModel
        {
            Id = role.Id,
            Name = role.Name
        });
    }

    public async Task<Result> UpdateRoleAsync(
        UpdateRoleCommand command,
        CancellationToken cancellation = default)
    {
        var role = await roleManager.FindByIdAsync(command.Id);

        if (role is null)
            Result.Failure(Error.Failure("Role.Update", ErrorMessages.ROLE_NOT_FOUND));

        role.Name = command.name;

        var result = await roleManager.UpdateAsync(role);

        return result.Succeeded
            ? Result.Success()
            : Result.Failure(Error.Failure("Role.Delete", ErrorMessages.UNABLE_UPDATE_ROLE));
    }



    public async Task<Result> AddOrRemoveClaimsToRoleAsync(
        string roleId, 
        List<string> claims, 
        CancellationToken cancellation = default)
    {
        var role = await roleManager.FindByIdAsync(roleId);

        if (role is null)
            Result.Failure(Error.Failure("Role.Update", ErrorMessages.ROLE_NOT_FOUND));

        var result = await RemoveAndAddPermissionAsync(identityContext, role, claims, cancellation);

        return result ? Result.Success() : Result.Failure(Error.Failure("Role.Permission", ErrorMessages.UNABLE_UPDATE_PERMISSION));
    }

    public Result<IList<TreeNodeModel<Guid>>> GetAllPermissions(string roleId) 
    {
        return Result.Success(PermissionHelper.MapPermissionsToTree());
    }

    private async Task<bool> RemoveAndAddPermissionAsync(
        IdentityContext identityContext,
        IdentityRole role,
        List<string> permissions,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var existedPermissions = await identityContext.RoleClaims
            .Where(x => x.RoleId == role.Id)
            .ToListAsync(cancellationToken);

            identityContext.RoleClaims.RemoveRange(existedPermissions);

            var newPermissions = permissions.Select(x => new IdentityRoleClaim<string>
            {
                RoleId = role.Id,
                ClaimType = CustomClaimTypes.Permission,
                ClaimValue = x
            });

            identityContext.RoleClaims.AddRange(newPermissions);

            await identityContext.SaveChangesAsync(cancellationToken);

            return true;

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Fail to update role permission");
            return true; 
        }

    }
}
