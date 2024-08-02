using CleanArchitechture.Application.Common.Extensions;
using CleanArchitechture.Application.Common.Models;
using CleanArchitechture.Application.Features.Admin.AppMenus.Queries;
using CleanArchitechture.Application.Features.Common.Queries;
using CleanArchitechture.Application.Features.Lookups.Commands;
using CleanArchitechture.Application.Features.Lookups.Queries;

namespace CleanArchitechture.Web.Endpoints;

public sealed class Lookups : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
           .RequireAuthorization()
           .MapPost(GetAll, "GetAll", "GetLookups")
           //.MapGet(Get, "Get/{id:Guid}", "GetLookup")
           .MapPost(Create, "Create", "CreateLookup")
           .MapPut(Update, "Update", "UpdateLookup")
           .MapDelete(Delete, "Delete/{id:Guid}", "DeleteLookup")
           .MapGet("Get/{id:Guid}", Get)
            .WithDescription("Get Single Entity of Lookup")
            .WithName("GetLookup")
            .Produces<LookupModel>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    [ProducesResponseType(typeof(PaginatedResponse<LookupModel>), StatusCodes.Status200OK)]
    public async Task<IResult> GetAll(ISender sender, [FromBody] GetLookupListQuery query)
    {
        var result = await sender.Send(query);

        if (!query.IsInitialLoaded)
        {
            var parentSelectList = await sender.Send(new GetSelectListQuery(
                Sql: SelectListSqls.GetLookupParentSelectListSql,
                Parameters: new { },
                Key: CacheKeys.Lookup_All_SelectList,
                AllowCacheList: false)
            );
            result.Value.OptionsDataSources.Add("parentSelectList", parentSelectList.Value);
            result.Value.OptionsDataSources.Add("statusSelectList", UtilityExtensions.GetActiveInactiveSelectList());
        }

        return TypedResults.Ok(result.Value);
    }

    //[ProducesResponseType(typeof(LookupModel), StatusCodes.Status200OK)]
    public async Task<IResult> Get(ISender sender, [FromRoute] Guid id)
    {
        var result = await sender.Send(new GetLookupByIdQuery(id));

        var parentSelectList = await sender.Send(new GetSelectListQuery(
                Sql: SelectListSqls.GetLookupSelectListSql,
                Parameters: new { },
                Key: CacheKeys.Lookup_All_SelectList,
                AllowCacheList: false)
            );
        result?.Value?.OptionsDataSources.Add("parentSelectList", parentSelectList.Value);
        result?.Value?.OptionsDataSources.Add("subjectSelectList", GetSubjectList());
        result?.Value?.OptionsDataSources.Add("subjectRadioSelectList", GetSubjectList());
        result?.Value?.OptionsDataSources.Add("multiParentSelectList", parentSelectList.Value);
        
        var appMenuTreeList = await sender.Send(new GetAppMenuTreeSelectList()).ConfigureAwait(false);
        result?.Value.OptionsDataSources.Add("appMenuTreeList", appMenuTreeList.Value);
        result?.Value.OptionsDataSources.Add("singleMenuTreeList", appMenuTreeList.Value);

        var parentTreeSelectList = await sender.Send(new GetAppMenuTreeSelectList());
        result?.Value?.OptionsDataSources.Add("treeSelectMenuseSelectList", parentTreeSelectList.Value);
        result?.Value?.OptionsDataSources.Add("treeSelectSingleMenuSelectList", parentTreeSelectList.Value);

        return TypedResults.Ok(result.Value);

    }

    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Create(ISender sender, [FromBody] CreateLookupCommand command)
    //public async Task<IResult> CreateLookup(ISender sender, [FromBody] CreateLookupCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
             onSuccess: () => Results.CreatedAtRoute(nameof(Get), new { id = result.Value }),
             onFailure: result.ToProblemDetails);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> Update(ISender sender, [FromBody] UpdateLookupCommand command)
    //public async Task<IResult> UpdateLookup(ISender sender, [FromBody] UpdateLookupCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
             onSuccess: () => Results.Ok(),
             onFailure: result.ToProblemDetails);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Delete(ISender sender, [FromRoute] Guid id)
    //public async Task<IResult> DeleteLookup(ISender sender, [FromRoute] Guid id)
    {
        var result = await sender.Send(new DeleteLookupCommand(id));

        return result.Match(
             onSuccess: Results.NoContent,
             onFailure: result.ToProblemDetails);
    }

    private List<SelectListModel> GetSubjectList()
    {
        return [
                new SelectListModel{ Name = "Accounting", Id = "A" },
                new SelectListModel{ Name = "Marketing", Id = "M" },
                new SelectListModel{ Name = "Production", Id = "P" },
                new SelectListModel{ Name = "Research", Id = "R" }
         ];
    }
}
