using System.Text.Json.Serialization;
using CleanArchitechture.Application.Common.Abstractions.Caching;
using CleanArchitechture.Application.Common.Abstractions.Identity;

namespace CleanArchitechture.Application.Features.Admin.AppUsers.Commands;

public record CreateAppUserCommand(
     string Username,
     string Password,
     string Email,
     string FirstName,
     string LastName,
     string PhoneNumber,
     string PhotoUrl,
     bool IsActive,
     List<string>? Roles
    ) : ICacheInvalidatorCommand<string>
{
    [JsonIgnore]
    public string CacheKey => CacheKeys.AppUser;
}

internal sealed class CreateAppUserCommandHandler(IIdentityService identityService) : ICommandHandler<CreateAppUserCommand, string>
{
    public async Task<Result<string>> Handle(CreateAppUserCommand request, CancellationToken cancellationToken)
    {
        return await identityService.CreateUserAsync(request, cancellationToken);
    }
}


