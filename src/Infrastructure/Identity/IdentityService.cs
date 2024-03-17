using Application.Constants;
using CleanArchitechture.Application.Common.Abstractions.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace CleanArchitechture.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
    }

    public async Task<string?> GetUserNameAsync(string userId, CancellationToken cancellation = default)
    {
        var user = await _userManager.Users
            .FirstAsync(u => u.Id == userId, cancellation);

        return user.UserName;
    }

    public async Task<Result<string>> CreateUserAsync(
        string userName,
        string password,
        CancellationToken cancellation = default)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = userName,
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            return Result.Failure<string>(Error.Failure("User.Create", ErrorMessages.UNABLE_CREATE_USER));
        }

        return Result.Success(user.Id);
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

        if(user is null) return Result.Failure(Error.NotFound(nameof(user), ErrorMessages.USER_NOT_FOUND));

        var result = await _userManager.DeleteAsync(user!);

        if(!result.Succeeded)
        {
            return Result.Failure(Error.Failure("User.Delete", ErrorMessages.UNABLE_DELETE_USER));
        }

        return Result.Success();
    }

    public async Task<IDictionary<string, string?>> FetchUsers(string roleName, CancellationToken cancellation = default)
    {
        var result = await _userManager.GetUsersInRoleAsync(roleName);

        return result?.ToDictionary(x => x.UserName, y => $"{y.FirstName} {y.LastName}");
        //return result?.ToDictionary(x => x.UserName, y => $"");
    }
}
