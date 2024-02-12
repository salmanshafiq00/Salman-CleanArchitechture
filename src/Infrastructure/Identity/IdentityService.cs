using CleanArchitechture.Application.Common.Interfaces.Identity;
using CleanArchitechture.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

        return result.ToApplicationResult<string>(user.Id);
    }

    public async Task<bool> IsInRoleAsync(string userId, string role, CancellationToken cancellation = default)
    {
        var user = await _userManager.Users
            .SingleOrDefaultAsync(u => u.Id == userId, cancellation);

        Guard.Against.NotFound(userId, nameof(userId));

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName, CancellationToken cancellation = default)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        if (user is null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<Result> DeleteUserAsync(string userId, CancellationToken cancellation = default)
    {
        var user = await _userManager.Users
            .SingleOrDefaultAsync(u => u.Id == userId, cancellation);

        Guard.Against.NotFound(userId, nameof(userId));

        var result = await _userManager.DeleteAsync(user!);

        return result.ToApplicationResult();
    }

    public async Task<IDictionary<string, string?>> FetchUsers(string roleName, CancellationToken cancellation = default)
    {
        var result = await _userManager.GetUsersInRoleAsync(roleName);

        return result?.ToDictionary(x => x.UserName, y => $"{y.FirstName} {y.LastName}");
        //return result?.ToDictionary(x => x.UserName, y => $"");
    }
}
