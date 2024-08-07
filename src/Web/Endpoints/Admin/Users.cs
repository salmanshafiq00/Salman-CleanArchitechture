﻿using CleanArchitechture.Application.Common.Extensions;
using CleanArchitechture.Application.Features.Admin.AppUsers.Commands;
using CleanArchitechture.Application.Features.Admin.AppUsers.Queries;
using CleanArchitechture.Application.Features.Common.Queries;

namespace CleanArchitechture.Web.Endpoints.Admin;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(GetAll, "GetAll", "GetUsers")
            .MapGet(Get, "Get/{id}", "GetUser")
            .MapPost(Create,"Create", "CreateUser")
            .MapPut(Update, "Update", "UpdateUser")
            .MapPost(AddToRoles, "AddToRoles");

    }

    [ProducesResponseType(typeof(PaginatedResponse<AppUserModel>), StatusCodes.Status200OK)]
    public async Task<IResult> GetAll(ISender sender, [FromBody] GetAppUserListQuery query)
    {
        var result = await sender.Send(query);

        if (!query.IsInitialLoaded)
        {
            var roleSelectList = await sender.Send(new GetSelectListQuery(
                Sql: SelectListSqls.GetRoleSelectListSql,
                Parameters: new { },
                Key: CacheKeys.Role_All_SelectList,
                AllowCacheList: false)
            );
            result.Value.OptionsDataSources.Add("roleSelectList", roleSelectList.Value);
            result.Value.OptionsDataSources.Add("statusSelectList", UtilityExtensions.GetActiveInactiveSelectList());
        }

        return TypedResults.Ok(result.Value);
    }

    [ProducesResponseType(typeof(AppUserModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Get(ISender sender, [FromRoute] string id)
    {
        var result = await sender.Send(new GetAppUserByIdQuery(id));

        var roleSelectList = await sender.Send(new GetSelectListQuery(
                Sql: SelectListSqls.GetRoleSelectListSql,
                Parameters: new { },
                Key: CacheKeys.Role_All_SelectList,
                AllowCacheList: false)
            );
        result.Value.OptionsDataSources.Add("roleSelectList", roleSelectList.Value);

        return result.Match(
             onSuccess: () => Results.Ok(result.Value),
             onFailure: result.ToProblemDetails);
    }

    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Create(ISender sender, [FromBody] CreateAppUserCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
             onSuccess: () => Results.Ok(result.Value),
             onFailure: result.ToProblemDetails);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Update(ISender sender, [FromBody] UpdateAppUserCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
             onSuccess: () => Results.NoContent(),
             onFailure: result.ToProblemDetails);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> AddToRoles(ISender sender, [FromBody] AddToRolesCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
             onSuccess: () => Results.NoContent(),
             onFailure: result.ToProblemDetails);
    }


}
