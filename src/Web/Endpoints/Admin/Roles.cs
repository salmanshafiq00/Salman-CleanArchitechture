﻿using CleanArchitechture.Application.Common.Abstractions;
using CleanArchitechture.Application.Common.Models;
using CleanArchitechture.Application.Features.Admin.AppMenus.Queries;
using CleanArchitechture.Application.Features.Admin.Roles.Commands;
using CleanArchitechture.Application.Features.Admin.Roles.Queries;
using CleanArchitechture.Infrastructure.Communications;
using Microsoft.AspNetCore.SignalR;

namespace CleanArchitechture.Web.Endpoints.Admin;

public class Roles : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);

        group.MapPost("GetAll", GetAll)
            .WithName("GetRoles")
            .Produces<PaginatedResponse<RoleModel>>(StatusCodes.Status200OK);

        group.MapGet("Get/{id}", Get)
            .WithName("GetRole")
            .Produces<RoleModel>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPost("Create", Create)
            .WithName("CreateRole")
            .Produces<string>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPut("Update", Update)
            .WithName("UpdateRole")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapGet("GetRolePermissions/{id}", GetRolePermissions)
            .WithName("GetRolePermissions")
            .Produces<IList<TreeNodeModel>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }

    private async Task<IResult> GetAll(ISender sender, [FromBody] GetRoleListQuery query)
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result.Value);
    }

    private async Task<IResult> Get(ISender sender, [FromRoute] string id)
    {
        var result = await sender.Send(new GetRoleByIdQuery(id));

        var permissionNodeList = await sender.Send(new GetPermissionTreeSelectListQuery()).ConfigureAwait(false);
        result.Value.OptionsDataSources["permissionNodeList"] = permissionNodeList.Value;

        var appMenuTreeList = await sender.Send(new GetAppMenuTreeSelectList()).ConfigureAwait(false);
        result.Value.OptionsDataSources["appMenuTreeList"] = appMenuTreeList.Value;

        return result.Match(
            onSuccess: () => Results.Ok(result.Value),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> Create(ISender sender, [FromBody] CreateRoleCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.Ok(result.Value),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> Update(
        ISender sender, 
        IHubContext<NotificationHub, INotificationHub> signalrContext, 
        [FromBody] UpdateRoleCommand command)
    {
        var result = await sender.Send(command);

        if (result.IsSuccess)
        {
            await signalrContext.Clients.All.ReceiveRolePermissionNotify();
        }

        return result.Match(
            onSuccess: () => Results.NoContent(),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> GetRolePermissions(ISender sender, [FromRoute] string id)
    {
        var result = await sender.Send(new GetPermissionsByRoleQuery(id));

        return result.Match(
            onSuccess: () => Results.Ok(result.Value),
            onFailure: result.ToProblemDetails);
    }
}
