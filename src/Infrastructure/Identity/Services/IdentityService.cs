using Application.Constants;
using CleanArchitechture.Application.Common.Abstractions.Identity;
using CleanArchitechture.Application.Features.Admin.AppUsers.Commands;
using CleanArchitechture.Application.Features.Admin.AppUsers.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace CleanArchitechture.Infrastructure.Identity.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;
    private readonly IdentityContext _identityContext;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService,
        IdentityContext identityContext)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
        _identityContext = identityContext;
    }

    public async Task<string?> GetUserNameAsync(string userId, CancellationToken cancellation = default)
    {
        var user = await _userManager.Users
            .FirstAsync(u => u.Id == userId, cancellation);

        return user.UserName;
    }

    public async Task<Result<string>> CreateUserAsync(
        CreateAppUserCommand command,
        CancellationToken cancellation = default)
    {
        var user = new ApplicationUser
        {
            UserName = command.Username,
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName,
            IsActive = command.IsActive,
            PhotoUrl = command.PhotoUrl,
            PhoneNumber = command.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, command.Password);

        if (result.Succeeded && command.Roles?.Count > 0)
        {
            _identityContext.UserRoles.AddRange(command.Roles.Select(x => new IdentityUserRole<string>
            {
                UserId = user.Id,
                RoleId = x
            }));

            await _identityContext.SaveChangesAsync(cancellation);
        }

        return result.ToApplicationResult<string>(user.Id);
    }

    public async Task<Result> UpdateUserAsync(
       UpdateAppUserCommand command,
       CancellationToken cancellation = default)
    {

        var user = await _identityContext.Users
            .SingleOrDefaultAsync(u => u.Id == command.Id, cancellation)
            .ConfigureAwait(false);

        if (user is null)
            return Result.Failure(Error.Failure("User.Update", ErrorMessages.USER_NOT_FOUND));

        user.UserName = command.Username;
        user.Email = command.Email;
        user.FirstName = command.FirstName;
        user.LastName = command.LastName;
        user.IsActive = command.IsActive;
        user.PhotoUrl = command.PhotoUrl;
        user.PhoneNumber = command.PhoneNumber;

        if (command.Roles?.Count > 0)
        {
            await DeleteAndAddUserRoles(command.Roles, user, cancellation);
        }


        await _identityContext.SaveChangesAsync(cancellation);

        return Result.Success();
    }

    private async Task DeleteAndAddUserRoles(
        List<string> roles, 
        ApplicationUser user, 
        CancellationToken cancellation)
    {
        var existedRoles = await _identityContext.UserRoles
                    .Where(x => x.UserId.ToLower() == user.Id)
                    .ToListAsync(cancellation);

        _identityContext.UserRoles.RemoveRange(existedRoles);

        _identityContext.UserRoles
            .AddRange(roles.Select(x => new IdentityUserRole<string>
            {
                UserId = user.Id,
                RoleId = x
            }));
    }

    public async Task<Result<AppUserModel>> GetUserAsync(
      string id,
      CancellationToken cancellation = default)
    {
        var user = await _userManager.Users
            .SingleOrDefaultAsync(u => u.Id == id, cancellation);

        if (user is null)
            return Result.Failure<AppUserModel>(Error.Failure("User.Get", ErrorMessages.USER_NOT_FOUND));

        var appUser = new AppUserModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsActive = user.IsActive,
            PhotoUrl = user.PhotoUrl,
            Username = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };

        appUser.Roles = await _userManager.GetRolesAsync(user);


        return Result.Success(appUser);
    }

    public async Task<Result> IsInRoleAsync(string userId, string role, CancellationToken cancellation = default)
    {
        var user = await _userManager.Users
            .SingleOrDefaultAsync(u => u.Id == userId, cancellation);

        if (user is null) return Result.Failure(Error.NotFound(nameof(user), ErrorMessages.USER_NOT_FOUND));

        return await _userManager.IsInRoleAsync(user, role)
            ? Result.Success()
            : Result.Failure(Error.Forbidden(nameof(ErrorType.Forbidden), "You have no permission to access the resource"));
    }

    public async Task<Result> AuthorizeAsync(string userId, string policyName, CancellationToken cancellation = default)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        if (user is null) return Result.Failure(Error.NotFound(nameof(user), ErrorMessages.USER_NOT_FOUND));

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded
            ? Result.Success()
            : Result.Failure(Error.Unauthorized(nameof(ErrorType.Unauthorized), string.Empty));
    }

    public async Task<Result> DeleteUserAsync(string userId, CancellationToken cancellation = default)
    {
        var user = await _userManager.Users
        .SingleOrDefaultAsync(u => u.Id == userId, cancellation);

        if (user is null) return Result.Failure(Error.NotFound(nameof(user), ErrorMessages.USER_NOT_FOUND));

        var result = await _userManager.DeleteAsync(user!);

        if (!result.Succeeded)
        {
            return Result.Failure(Error.Failure("User.Delete", ErrorMessages.UNABLE_DELETE_USER));
        }

        return Result.Success();
    }

    public async Task<IDictionary<string, string?>> GetUsersByRole(
        string roleName,
        CancellationToken cancellation = default)
    {
        var result = await _userManager.GetUsersInRoleAsync(roleName);

        return result?.ToDictionary(x => x.UserName, y => $"{y.FirstName} {y.LastName}")!;
        //return result?.ToDictionary(x => x.UserName, y => $"");
    }

    public async Task<Result> AddToRolesAsync(
        AddToRolesCommand command,
        CancellationToken cancellation = default)
    {
        var user = await _userManager.FindByIdAsync(command.Id);

        if (user is null) return Result.Failure(Error.NotFound(nameof(user), ErrorMessages.USER_NOT_FOUND));

        var result = await _userManager.AddToRolesAsync(user, command.RoleNames);

        return result.Succeeded
            ? Result.Success()
            : Result.Failure(Error.Unauthorized(nameof(ErrorType.Unauthorized), string.Empty));
    }
}
