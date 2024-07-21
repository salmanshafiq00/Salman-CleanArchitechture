using CleanArchitechture.Application.Common.Models;
using CleanArchitechture.Application.Features.Admin.AppMenus.Queries;
using CleanArchitechture.Application.Features.Admin.Roles.Queries;

namespace CleanArchitechture.Web.Endpoints;

public class TreeNodeLists : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetAllPermissionNodeList, "GetAllPermissionNodeList")
            .MapGet(GetAllAppMenuTreeSelectList, "GetAllAppMenuTreeSelectList");
    }

    [ProducesResponseType(typeof(List<TreeNodeModel>), StatusCodes.Status200OK)]
    public async Task<List<TreeNodeModel>> GetAllPermissionNodeList(ISender sender, [FromQuery] bool? allowCache = null)
    {
        var result = await sender.Send(new GetPermissionTreeSelectListQuery());
        return result.Value;
    }

    [ProducesResponseType(typeof(List<TreeNodeModel>), StatusCodes.Status200OK)]
    public async Task<List<TreeNodeModel>> GetAllAppMenuTreeSelectList(ISender sender, [FromQuery] bool? allowCache = null)
    {
        var result = await sender.Send(new GetAppMenuTreeSelectList());
        return result.Value;
    }
}
