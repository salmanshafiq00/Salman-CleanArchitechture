using System.Text.Json.Serialization;
using CleanArchitechture.Domain.Admin;

namespace CleanArchitechture.Application.Features.Admin.AppMenus.Commands;

public record CreateAppMenuCommand(
    string Label,
    string RouterLink,
    string Icon,
    bool IsActive,
    bool Visible,
    int OrderNo,
    string Tooltip,
    string Description,
    Guid MenuTypeId,
    Guid? ParentId = null) : ICacheInvalidatorCommand<Guid>
{
    [JsonIgnore]
    public string CacheKey => CacheKeys.AppMenu;
}

internal sealed class CreateAppMenuQueryHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<CreateAppMenuCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateAppMenuCommand request, CancellationToken cancellationToken)
    {
        var entity = new AppMenu
        {
            Label = request.Label,
            RouterLink = request.RouterLink,
            Icon = request.Icon,
            Tooltip = request.Tooltip,
            IsActive = request.IsActive,
            OrderNo = request.OrderNo,
            Visible = request.Visible,
            Description = request.Description,
            ParentId = request.ParentId,
            MenuTypeId = request.MenuTypeId
        };

        dbContext.AppMenus.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(entity.Id);
    }
}
