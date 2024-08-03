using System.Diagnostics;
using System.Security.Claims;
using Application.Constants;
using CleanArchitechture.Application.Common.Abstractions.Caching;
using CleanArchitechture.Application.Common.Abstractions.Identity;
using CleanArchitechture.Application.Features.Admin.AppUsers.Commands;
using CleanArchitechture.Application.Features.Admin.AppUsers.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace CleanArchitechture.Infrastructure.Identity.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;
    private readonly IdentityContext _identityContext;
    private readonly IDistributedCacheService _cacheService;
    private readonly ILogger<IdentityService> _logger;
    private readonly Stopwatch _timer;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService,
        IdentityContext identityContext,
        IDistributedCacheService cacheService,
        ILogger<IdentityService> logger)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
        _identityContext = identityContext;
        _cacheService = cacheService;
        _logger = logger;
        _timer = new Stopwatch();
    }

    public async Task<string?> GetUserNameAsync(string userId, CancellationToken cancellation = default)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .FirstAsync(u => u.Id == userId, cancellation);

        return user?.UserName;
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

        return result.ToApplicationResult(user.Id);
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
        await _identityContext.UserRoles
                    .Where(x => x.UserId == user.Id)
                    .ExecuteDeleteAsync(cancellation);

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
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == id, cancellation);

        if (user is null)
            return Result.Failure<AppUserModel>(Error.Failure("User.Found", ErrorMessages.USER_NOT_FOUND));

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

        appUser.Roles = await _identityContext.UserRoles
            .AsNoTracking()
            .Where(x => x.UserId == id)
            .Select(x => x.RoleId)
            .ToListAsync(cancellation);

        return Result.Success(appUser);
    }

    public async Task<Result> IsInRoleAsync(string userId, string role, CancellationToken cancellation = default)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == userId, cancellation);

        if (user is null) return Result.Failure(Error.NotFound(nameof(user), ErrorMessages.USER_NOT_FOUND));
        
        return await _userManager.IsInRoleAsync(user, role)
            ? Result.Success()
            : Result.Failure(Error.Forbidden(nameof(ErrorType.Forbidden), "You have no permission to access the resource"));
    }

    public async Task<Result> AuthorizeAsync(string userId, string policyName, CancellationToken cancellation = default)
    {
        _timer.Start();

        var user = _userManager.Users
            .AsNoTracking()
            .SingleOrDefault(u => u.Id == userId);

        if (user is null) return Result.Failure(Error.NotFound(nameof(user), ErrorMessages.USER_NOT_FOUND));

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        _logger.LogInformation("Current DateTime: {Now}", DateTime.Now);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        _timer.Stop();

        var ellapsed = _timer.ElapsedMilliseconds;

        _logger.LogInformation("Elapsed Time: {Elapsed}ms", ellapsed);

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

        return result.Succeeded
            ? Result.Success()
            : Result.Failure(Error.Failure("User.Delete", ErrorMessages.UNABLE_DELETE_USER));
    }

    public async Task<IDictionary<string, string?>> GetUsersByRole(
        string roleName,
        CancellationToken cancellation = default)
    {
        var result = await _userManager.GetUsersInRoleAsync(roleName);

        return result?.ToDictionary(x => x.UserName, y => $"{y.FirstName} {y.LastName}")!;
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
