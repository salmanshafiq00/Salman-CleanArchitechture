using CleanArchitechture.Application.Common.Abstractions.Caching;

namespace CleanArchitechture.Application.Features.Admin.AppPages.Commands;

public record UpdateAppPageCommand(
    Guid Id,
    string Title,
    string SubTitle,
    string ComponentName,
    string AppPageLayout) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.AppPage;
}

internal sealed class UpdateAppPageCommandHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<UpdateAppPageCommand, Result>
{
    public async Task<Result> Handle(UpdateAppPageCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.AppPages
            .AsNoTracking()
            .FirstOrDefaultAsync(ap => ap.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            return Result.Failure(Error.NotFound("AppPage.NotFound", "AppPage not found"));
        }

        entity.Title = request.Title;
        entity.SubTitle = request.SubTitle;
        entity.ComponentName = request.ComponentName;
        entity.AppPageLayout = request.AppPageLayout;

        dbContext.AppPages.Update(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
