using CleanArchitechture.Application.Common.Abstractions.Caching;
using CleanArchitechture.Domain.Admin;

namespace CleanArchitechture.Application.Features.Admin.AppNotifications.Commands;

public record CreateAppNotificationCommand(
    string Title,
    string SubTitle,
    string ComponentName,
    string AppNotificationLayout) : ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.AppNotification;
    private class Mapper: Profile
    {
        public Mapper()
        {
            CreateMap<CreateAppNotificationCommand, AppNotification>();
        }
    }
}

internal sealed class CreateAppNotificationCommandHandler(IApplicationDbContext dbContext, IMapper mapper)
    : ICommandHandler<CreateAppNotificationCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateAppNotificationCommand request, CancellationToken cancellationToken)
    {
        var entity = mapper.Map<AppNotification>(request);
        dbContext.AppNotifications.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(entity.Id);
    }
}
