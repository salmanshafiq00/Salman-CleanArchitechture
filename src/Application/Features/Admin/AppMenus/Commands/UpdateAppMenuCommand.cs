using Application.Constants;

namespace CleanArchitechture.Application.Features.Admin.AppMenus.Commands;

public record UpdateAppMenuCommand(
    Guid Id,
    string Label,
    string RouterLink,
    string Icon,
    bool IsActive,
    bool Visible,
    int OrderNo,
    string Tooltip,
    string Description,
    Guid MenuTypeId,
    Guid? ParentId = null) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.AppMenu;
}

internal sealed class UpdateAppMenuCommandHandler(
    IApplicationDbContext dbContext)
    : ICommandHandler<UpdateAppMenuCommand>
{
    public async Task<Result> Handle(UpdateAppMenuCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.AppMenus.FindAsync(request.Id, cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        entity.Label = request.Label;
        entity.RouterLink = request.RouterLink;
        entity.Icon = request.Icon;
        entity.Tooltip = request.Tooltip;
        entity.IsActive = request.IsActive;
        entity.OrderNo = request.OrderNo;
        entity.Visible = request.Visible;
        entity.Description = request.Description;
        entity.ParentId = request.ParentId;
        entity.MenuTypeId = request.MenuTypeId;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
