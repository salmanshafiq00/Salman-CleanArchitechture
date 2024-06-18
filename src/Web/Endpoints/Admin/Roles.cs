using CleanArchitechture.Application.Common.DapperQueries;
using CleanArchitechture.Application.Common.Models;
using CleanArchitechture.Application.Features.Admin.Roles.Commands;
using CleanArchitechture.Application.Features.Admin.Roles.Queries;
using CleanArchitechture.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitechture.Web.Endpoints.Admin;

public class Roles : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(GetRoles)
            .MapGet(GetRole, "GetRole/{id}")
            .MapPost(Create, "Create")
            .MapPut(UpdateRole, "UpdateRole")
            .MapGet(GetRolePermissions, "GetRolePermissions/{id}");
    }

    [ProducesResponseType(typeof(PaginatedResponse<RoleModel>), StatusCodes.Status200OK)]
    public async Task<IResult> GetRoles(ISender sender, [FromBody] GetRoleListQuery query)
    {
        var result = await sender.Send(query);

        return TypedResults.Ok(result.Value);
    }

    [ProducesResponseType(typeof(RoleModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetRole(ISender sender, [FromRoute] string id)
    {
        var result = await sender.Send(new GetRoleByIdQuery(id));
        var permissionNodeList = await sender.Send(new GetPermissionNodeListQuery()).ConfigureAwait(false);
        result.Value.OptionsDataSources["permissionNodeList"] = permissionNodeList.Value;

        return result.Match(
             onSuccess: () => Results.Ok(result.Value),
             onFailure: result.ToProblemDetails);
    }

    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Create(ISender sender, [FromBody] CreateRoleCommand command)
    {
        
        var result = await sender.Send(command);

        return result.Match(
             onSuccess: () => Results.Ok(result.Value),
             onFailure: result.ToProblemDetails);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> UpdateRole(ISender sender, [FromBody] UpdateRoleCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
             onSuccess: () => Results.NoContent(),
             onFailure: result.ToProblemDetails);
    }



    [ProducesResponseType(typeof(IList<TreeNodeModel<Guid>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetRolePermissions(ISender sender, [FromRoute] string id)
    {
        var result = await sender.Send(new GetPermissionsByRoleQuery(id));

        return result.Match(
             onSuccess: () => Results.Ok(result.Value),
             onFailure: result.ToProblemDetails);
    }
}
