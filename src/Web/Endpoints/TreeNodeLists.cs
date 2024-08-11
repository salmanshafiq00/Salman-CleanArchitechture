using CleanArchitechture.Application.Common.Models;
using CleanArchitechture.Application.Features.Admin.AppMenus.Queries;
using CleanArchitechture.Application.Features.Admin.Roles.Queries;

namespace CleanArchitechture.Web.Endpoints;

public class TreeNodeLists : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);

        group.MapGet("GetAllPermissionNodeList", GetAllPermissionNodeList)
             .WithName("GetAllPermissionNodeList")
             .Produces<List<TreeNodeModel>>(StatusCodes.Status200OK);

        group.MapGet("GetAllAppMenuTreeSelectList", GetAllAppMenuTreeSelectList)
             .WithName("GetAllAppMenuTreeSelectList")
             .Produces<List<TreeNodeModel>>(StatusCodes.Status200OK);
    }

    private async Task<List<TreeNodeModel>> GetAllPermissionNodeList(ISender sender, [FromQuery] bool? allowCache = null)
    {
        var result = await sender.Send(new GetPermissionTreeSelectListQuery());
        return result.Value;
    }

    private async Task<List<TreeNodeModel>> GetAllAppMenuTreeSelectList(ISender sender, [FromQuery] bool? allowCache = null)
    {
        var result = await sender.Send(new GetAppMenuTreeSelectList());
        return result.Value;
    }
}
