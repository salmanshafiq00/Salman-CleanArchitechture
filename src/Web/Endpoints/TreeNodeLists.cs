using CleanArchitechture.Application.Common.Models;
using CleanArchitechture.Application.Features.Admin.Roles.Queries;
using CleanArchitechture.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitechture.Web.Endpoints;

public class TreeNodeLists : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetAllPermissionNodeList, "GetAllPermissionNodeList");
    }

    [ProducesResponseType(typeof(List<TreeNodeModel>), StatusCodes.Status200OK)]
    public async Task<List<TreeNodeModel>> GetAllPermissionNodeList(ISender sender, [FromQuery] bool? allowCache = null)
    {
        var result = await sender.Send(new GetPermissionNodeListQuery());
        return result.Value;
    }
}
