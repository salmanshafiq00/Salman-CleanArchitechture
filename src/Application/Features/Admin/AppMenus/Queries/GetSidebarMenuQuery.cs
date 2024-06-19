using System.Data;
using System.Text.Json.Serialization;
using CleanArchitechture.Domain.Admin;

namespace CleanArchitechture.Application.Features.Admin.AppMenus.Queries;

public record GetSidebarMenuQuery
    : ICacheableQuery<List<SidebarMenuModel>>
{
    [JsonIgnore]
    public string CacheKey => CacheKeys.AppMenu_Tree_SelectList;

    public bool? AllowCache => false;

    public TimeSpan? Expiration => null;
}

public class SidebarMenuModel
{
    public Guid Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public bool Visible { get; set; }
    public string Tooltip { get; set; } = string.Empty;
    public int OrderNo { get; set; }
    public Guid? ParentId { get; set; }
    public IList<SidebarMenuModel>? Children { get; set; } = [];
}

internal sealed class GetSidebarMenuQueryHandler(ISqlConnectionFactory sqlConnection)
    : IQueryHandler<GetSidebarMenuQuery, List<SidebarMenuModel>>
{
    public async Task<Result<List<SidebarMenuModel>>> Handle(GetSidebarMenuQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT 
                M.Id AS {nameof(SidebarMenuModel.Id)}, 
                M.ParentId AS {nameof(SidebarMenuModel.ParentId)}, 
                M.Label AS {nameof(SidebarMenuModel.Label)}, 
                M.Url AS {nameof(SidebarMenuModel.Url)}, 
                M.Icon AS {nameof(SidebarMenuModel.Icon)}, 
                M.Visible AS {nameof(SidebarMenuModel.Visible)},
                M.Tooltip AS {nameof(SidebarMenuModel.Tooltip)},
                M.OrderNo AS {nameof(SidebarMenuModel.OrderNo)}
            FROM dbo.AppMenus AS M
            WHERE M.IsActive = 1
            ORDER BY M.OrderNo
            """;

        var appMenus = await connection.QueryAsync<SidebarMenuModel>(sql);

        var lookup = appMenus.ToLookup(x => x.ParentId);

        IList<SidebarMenuModel> BuildTree(Guid? parentId)
        {
            return lookup[parentId]
                .Select(x => new SidebarMenuModel
                {
                    Label = x.Label,
                    Url = x.Url,
                    Icon = x.Icon,
                    Visible = x.Visible,
                    Tooltip = x.Tooltip,
                    OrderNo = x.OrderNo,
                    Children = BuildTree(x.Id)
                })
                .ToList();
        }

        var tree = BuildTree(null);

        return Result.Success(tree.ToList());

    }
}





