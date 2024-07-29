using CleanArchitechture.Application.Common.Abstractions.Caching;
using CleanArchitechture.Domain.Admin;

namespace CleanArchitechture.Application.Features.Admin.AppPages.Commands;

public record CreateAppPageCommand(
    string Title,
    string SubTitle,
    string ComponentName,
    string AppPageLayout) : ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.AppPage;
    private class Mapper: Profile
    {
        public Mapper()
        {
            CreateMap<CreateAppPageCommand, AppPage>();
        }
    }
}

internal sealed class CreateAppPageCommandHandler(IApplicationDbContext dbContext, IMapper mapper)
    : ICommandHandler<CreateAppPageCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateAppPageCommand request, CancellationToken cancellationToken)
    {
        var entity = mapper.Map<AppPage>(request);
        dbContext.AppPages.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(entity.Id);
    }
}
