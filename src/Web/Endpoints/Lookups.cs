using CleanArchitechture.Application.Common.Caching;
using CleanArchitechture.Application.Common.Constants.CommonSqlConstants;
using CleanArchitechture.Application.Common.DapperQueries;
using CleanArchitechture.Application.Common.Extensions;
using CleanArchitechture.Application.Common.Models;
using CleanArchitechture.Application.Features.Common.Queries;
using CleanArchitechture.Application.Features.Lookups.Commands;
using CleanArchitechture.Application.Features.Lookups.Queries;
using CleanArchitechture.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitechture.Web.Endpoints;

public class Lookups : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
           //.RequireAuthorization()
           .MapPost(GetLookups, "GetLookups")
           .MapGet(GetLookup, "{id:Guid}")
           .MapPost(CreateLookup)
           .MapPut(UpdateLookup)
           .MapDelete(DeleteLookup, "{id:Guid}");
    }

    [ProducesResponseType(typeof(PaginatedResponse<LookupModel>), StatusCodes.Status200OK)]
    public async Task<IResult> GetLookups(ISender sender, [FromBody] GetLookupListQuery query)
    {
        var result = await sender.Send(query);

        if (!query.IsInitialLoaded)
        {
            var parentSelectList = await sender.Send(new GetSelectListQuery(
                Sql: SelectListSqls.GetLookupSelectListSql,
                Parameters: new { },
                Key: CacheKeys.Lookup_All_SelectList,
                AllowCacheList: false)
            );
            result.Value.OptionDataSources.Add("parentSelectList", parentSelectList.Value);
            result.Value.OptionDataSources.Add("statusSelectList", UtilityExtensions.GetActiveInactiveSelectList());
        }

        return TypedResults.Ok(result.Value);
    }

    [ProducesResponseType(typeof(LookupModel), StatusCodes.Status200OK)]
    public async Task<IResult> GetLookup(ISender sender, [FromRoute] Guid id)
    {
        var result = await sender.Send(new GetLookupByIdQuery(id));

        var parentSelectList = await sender.Send(new GetSelectListQuery(
                Sql: SelectListSqls.GetLookupSelectListSql,
                Parameters: new { },
                Key: CacheKeys.Lookup_All_SelectList,
                AllowCacheList: false)
            );
        result?.Value?.OptionDataSources.Add("parentSelectList", parentSelectList.Value);
        result?.Value?.OptionDataSources.Add("subjectSelectList", GetSubjectList());
        result?.Value?.OptionDataSources.Add("subjectRadioSelectList", GetSubjectList());

        return TypedResults.Ok(result.Value);

    }

    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> CreateLookup(ISender sender, [FromBody] CreateLookupCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
             onSuccess: () => Results.CreatedAtRoute(nameof(GetLookup), new { id = result.Value }, result.Value),
             onFailure: result.ToProblemDetails);
    }


    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> UpdateLookup(ISender sender, [FromBody] UpdateLookupCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
             onSuccess: () => Results.Ok(),
             onFailure: result.ToProblemDetails);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> DeleteLookup(ISender sender, [FromRoute] Guid id)
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
