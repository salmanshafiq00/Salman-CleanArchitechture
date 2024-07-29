using CleanArchitechture.Application.Common.Extensions;
using CleanArchitechture.Application.Features.Admin.AppMenus.Commands;
using CleanArchitechture.Application.Features.Admin.AppMenus.Queries;
using CleanArchitechture.Application.Features.Common.Queries;

namespace CleanArchitechture.Web.Endpoints.Admin;

public class AppMenus : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(GetAll, "GetAll", "GetAppMenus")
            .MapGet(GetSidebarMenus, "GetSidebarMenus")
            .MapGet(Get, "Get/{id}", "GetAppMenu")
            .MapPost(Create,"Create", "CreateMenu")
            .MapPut(Update, "Update", "UpdateMenu");

    }

    [ProducesResponseType(typeof(PaginatedResponse<AppMenuModel>), StatusCodes.Status200OK)]
    public async Task<IResult> GetAll(ISender sender, [FromBody] GetAppMenuListQuery query)
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
    public async Task<IResult> Get(ISender sender, [FromRoute] Guid id)
    {
        var result = await sender.Send(new GetAppMenuByIdQuery(id));

        var parentTreeSelectList = await sender.Send(new GetAppMenuTreeSelectList());
        result?.Value?.OptionsDataSources.Add("parentTreeSelectList", parentTreeSelectList.Value);

        var menuTypeSelectList = await sender.Send(new GetSelectListQuery(
        Sql: SelectListSqls.GetLookupDetailSelectListByDevCodeSql,
        Parameters: new { DevCode = 101 },
        Key: CacheKeys.LookupDetail_All_SelectList,
        AllowCacheList: false));
        result?.Value?.OptionsDataSources.Add("menuTypeSelectList", menuTypeSelectList.Value);

        return result.Match(
             onSuccess: () => Results.Ok(result.Value),
             onFailure: result.ToProblemDetails);
    }

    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Create(ISender sender, [FromBody] CreateAppMenuCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
             onSuccess: () => Results.Ok(result.Value),
             onFailure: result.ToProblemDetails);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Update(ISender sender, [FromBody] UpdateAppMenuCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
             onSuccess: () => Results.NoContent(),
             onFailure: result.ToProblemDetails);
    }

    [ProducesResponseType(typeof(List<SidebarMenuModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetSidebarMenus(ISender sender)
    {
        var result = await sender.Send(new GetSidebarMenuQuery());

        return result.Match(
             onSuccess: () => Results.Ok(result.Value),
             onFailure: result.ToProblemDetails);
    }

}
