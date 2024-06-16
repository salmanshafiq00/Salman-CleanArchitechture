using System.Text.Json.Serialization;
using CleanArchitechture.Application.Common.Abstractions.Identity;

namespace CleanArchitechture.Application.Features.Admin.AppUsers.Commands;

public record UpdateAppUserCommand(
     string Id,
     string Username,
     string Password,
     string Email,
     string FirstName,
     string LastName,
     string PhoneNumber,
     string PhotoUrl,
     bool IsActive
    ) : ICacheInvalidatorCommand
{
    [JsonIgnore]
    public string CacheKey => CacheKeys.AppUser;
}

internal sealed class UpdateAppUserCommandHandler(IIdentityService identityService) 
    : ICommandHandler<UpdateAppUserCommand>
{
    public async Task<Result> Handle(UpdateAppUserCommand request, CancellationToken cancellationToken)
    {
        return await identityService.UpdateUserAsync( request, cancellationToken );
    }
}
