using CleanArchitechture.Application.Common.Models;

namespace CleanArchitechture.Application.Common.Abstractions.Identity;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId, CancellationToken cancellation = default);

    Task<bool> IsInRoleAsync(string userId, string role, CancellationToken cancellation = default);

    Task<bool> AuthorizeAsync(string userId, string policyName, CancellationToken cancellation = default);

    Task<Result<string>> CreateUserAsync(string userName, string password, CancellationToken cancellation = default);

    Task<Result> DeleteUserAsync(string userId, CancellationToken cancellation = default);

    Task<IDictionary<string, string?>> FetchUsers(string roleName, CancellationToken cancellation = default);

}
