﻿using CleanArchitechture.Application.Features.Admin.AppUsers.Commands;
using CleanArchitechture.Application.Features.Admin.AppUsers.Queries;

namespace CleanArchitechture.Application.Common.Abstractions.Identity;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId, CancellationToken cancellation = default);

    Task<Result> IsInRoleAsync(string userId, string role, CancellationToken cancellation = default);

    Task<Result> AuthorizeAsync(string userId, string policyName, CancellationToken cancellation = default);

    Task<Result<string>> CreateUserAsync(CreateAppUserCommand command, CancellationToken cancellation = default);
    Task<Result> UpdateUserAsync(UpdateAppUserCommand command, CancellationToken cancellation = default);
    Task<Result<AppUserModel>> GetUserAsync(string id, CancellationToken cancellation = default);
    Task<Result> DeleteUserAsync(string userId, CancellationToken cancellation = default);
    Task<Result> AddToRolesAsync(AddToRolesCommand command, CancellationToken cancellation = default);

    Task<IDictionary<string, string?>> GetUsersByRole(string roleName, CancellationToken cancellation = default);

}
