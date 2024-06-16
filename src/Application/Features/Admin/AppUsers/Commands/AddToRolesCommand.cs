using System.Text.Json.Serialization;
using CleanArchitechture.Application.Common.Abstractions.Identity;

namespace CleanArchitechture.Application.Features.Admin.AppUsers.Commands;

public record AddToRolesCommand(
     string Id,
     List<string> RoleNames
    ) : ICacheInvalidatorCommand
{
    [JsonIgnore]
    public string CacheKey => CacheKeys.AppUser;
}

internal sealed class AddToRolesCommandHandler(IIdentityService identityService) 
    : ICommandHandler<AddToRolesCommand>
{
    public async Task<Result> Handle(AddToRolesCommand request, CancellationToken cancellationToken)
    {
        return await identityService.AddToRolesAsync(request, cancellationToken );
    }
}
