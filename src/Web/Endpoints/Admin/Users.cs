using CleanArchitechture.Application.Common.Abstractions.Caching;
using CleanArchitechture.Application.Common.Constants.CommonSqlConstants;
using CleanArchitechture.Application.Common.DapperQueries;
using CleanArchitechture.Application.Common.Extensions;
using CleanArchitechture.Application.Features.Admin.AppUsers.Commands;
using CleanArchitechture.Application.Features.Admin.AppUsers.Queries;
using CleanArchitechture.Application.Features.Common.Queries;
using CleanArchitechture.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace CleanArchitechture.Web.Endpoints.Admin;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(GetUsers)
            .MapGet(GetUser, "GetUser/{id}")
            .MapPost(CreateUser, "CreateUser")
            .MapPut(UpdateUser, "UpdateUser")
            .MapPost(AddToRoles, "AddToRoles");

    }

    [ProducesResponseType(typeof(PaginatedResponse<AppUserModel>), StatusCodes.Status200OK)]
    public async Task<IResult> GetUsers(ISender sender, [FromBody] GetAppUserListQuery query)
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
            result.Value.OptionDataSources.Add("roleSelectList", roleSelectList.Value);
            result.Value.OptionDataSources.Add("statusSelectList", UtilityExtensions.GetActiveInactiveSelectList());
        }

        return TypedResults.Ok(result.Value);
    }

    [ProducesResponseType(typeof(AppUserModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetUser(ISender sender, [FromRoute] string id)
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
    public async Task<IResult> CreateUser(ISender sender, [FromBody] CreateAppUserCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
             onSuccess: () => Results.Ok(result.Value),
             onFailure: result.ToProblemDetails);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> UpdateUser(ISender sender, [FromBody] UpdateAppUserCommand command)
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
