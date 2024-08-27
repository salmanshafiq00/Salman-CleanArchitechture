using CleanArchitechture.Application.Common.Abstractions.Caching;
using CleanArchitechture.Application.Common.Abstractions.Identity;

namespace CleanArchitechture.Application.Features.Admin.AppUsers.Commands;

public record ChangeUserPhotoCommand(string PhotoUrl
    ) : ICacheInvalidatorCommand
{
    [JsonIgnore]
    public string CacheKey => CacheKeys.AppUser;
}

internal sealed class ChangeUserPhotoCommandHandler(IIdentityService identityService, IUser user) 
    : ICommandHandler<ChangeUserPhotoCommand>
{
    public async Task<Result> Handle(ChangeUserPhotoCommand request, CancellationToken cancellationToken)
    {
        return await identityService.ChangePhotoAsync(user.Id, request.PhotoUrl, cancellationToken );
    }
}
