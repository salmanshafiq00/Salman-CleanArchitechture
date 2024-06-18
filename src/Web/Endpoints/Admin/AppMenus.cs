using CleanArchitechture.Application.Common.Caching;
using CleanArchitechture.Application.Common.Constants.CommonSqlConstants;
using CleanArchitechture.Application.Common.DapperQueries;
using CleanArchitechture.Application.Common.Extensions;
using CleanArchitechture.Application.Features.Admin.AppMenus.Commands;
using CleanArchitechture.Application.Features.Admin.AppMenus.Queries;
using CleanArchitechture.Application.Features.Common.Queries;
using CleanArchitechture.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace CleanArchitechture.Web.Endpoints.Admin;

public class AppMenus : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(GetAppMenus)
            .MapGet(GetAppMenu, "GetMenu/{id}")
            .MapPost(CreateMenu, "CreateMenu")
            .MapPut(UpdateMenu, "UpdateMenu");

    }

    [ProducesResponseType(typeof(PaginatedResponse<AppMenuModel>), StatusCodes.Status200OK)]
    public async Task<IResult> GetAppMenus(ISender sender, [FromBody] GetAppMenuListQuery query)
    {
        var result = await sender.Send(query);

        if (!query.IsInitialLoaded)
        {
            var roleSelectList = await sender.Send(new GetSelectListQuery(
                Sql: SelectListSqls.GetAppMenuSelectListSql,
                Parameters: new { },
                Key: CacheKeys.AppMenu_All_SelectList,
                AllowCacheList: false)
            );
            result.Value.OptionsDataSources.Add("parentSelectList", roleSelectList.Value);
            result.Value.OptionsDataSources.Add("statusSelectList", UtilityExtensions.GetActiveInactiveSelectList());
        }

        return TypedResults.Ok(result.Value);
    }

    [ProducesResponseType(typeof(AppMenuModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetAppMenu(ISender sender, [FromRoute] Guid id)
    {
        var result = await sender.Send(new GetAppMenuByIdQuery(id));

        var parentTreeSelectList = await sender.Send(new GetAppMenuTreeSelectList());
        result.Value.OptionsDataSources.Add("parentTreeSelectList", parentTreeSelectList.Value);

        return result.Match(
             onSuccess: () => Results.Ok(result.Value),
             onFailure: result.ToProblemDetails);
    }

    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> CreateMenu(ISender sender, [FromBody] CreateAppMenuCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
             onSuccess: () => Results.Ok(result.Value),
             onFailure: result.ToProblemDetails);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> UpdateMenu(ISender sender, [FromBody] UpdateAppMenuCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
             onSuccess: () => Results.NoContent(),
             onFailure: result.ToProblemDetails);
    }


}
