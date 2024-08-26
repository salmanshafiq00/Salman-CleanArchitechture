using CleanArchitechture.Application.Common.Extensions;
using CleanArchitechture.Application.Features.Common.Queries;
using CleanArchitechture.Application.Features.LookupDetails.Commands;
using CleanArchitechture.Application.Features.LookupDetails.Queries;

namespace CleanArchitechture.Web.Endpoints;

public class LookupDetailsTest : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);

        group.MapPost("GetAll", GetAll)
             .WithName("GetLookupDetails2")
             .Produces<PaginatedResponse<LookupDetailModel>>(StatusCodes.Status200OK);

        group.MapGet("Get/{id:Guid}", Get)
             .WithName("GetLookupDetail2")
             .Produces<LookupDetailModel>(StatusCodes.Status200OK);

        group.MapPost("Create", Create)
             .WithName("CreateLookupDetail2")
             .Produces<Guid>(StatusCodes.Status201Created)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPut("Update", Update)
             .WithName("UpdateLookupDetail2")
             .Produces(StatusCodes.Status200OK)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("Delete/{id:Guid}", Delete)
             .WithName("DeleteLookupDetail2")
             .Produces(StatusCodes.Status204NoContent)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPost("DeleteMultiple", DeleteMultiple)
             .WithName("DeleteMultiple2")
             .Produces(StatusCodes.Status204NoContent)
             .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }

    private async Task<IResult> GetAll(ISender sender, GetLookupDetailListQuery query)
    {
        var result = await sender.Send(query);
        if (!query.IsInitialLoaded)
        {
            var parentSelectList = await sender.Send(new GetSelectListQuery(
                Sql: SelectListSqls.GetLookupDetailParentSelectListSql,
                Parameters: new { },
                Key: CacheKeys.Lookup_All_SelectList,
                AllowCacheList: false)
            );

            result.Value.OptionsDataSources.Add("parentSelectList", parentSelectList.Value);
            result.Value.OptionsDataSources.Add("statusSelectList", UtilityExtensions.GetActiveInactiveSelectList());
        }
        return TypedResults.Ok(result.Value);
    }

    private async Task<IResult> Get(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetLookupDetailByIdQuery(id));

        var lookupSelectList = await sender.Send(new GetSelectListQuery(
            Sql: SelectListSqls.GetLookupSelectListSql,
            Parameters: new { },
            Key: CacheKeys.Lookup_All_SelectList,
            AllowCacheList: false));

        result.Value.OptionsDataSources.Add("lookupSelectList", lookupSelectList.Value);

        var parentSelectList = await sender.Send(new GetSelectListQuery(
            Sql: SelectListSqls.GetLookupDetailSelectListSql,
            Parameters: new { },
            Key: CacheKeys.LookupDetail_All_SelectList,
            AllowCacheList: false));

        result.Value.OptionsDataSources.Add("parentSelectList", parentSelectList.Value);

        return TypedResults.Ok(result.Value);
    }

    private async Task<IResult> Create(ISender sender, [FromBody] CreateLookupDetailCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.CreatedAtRoute("GetLookupDetail", new { id = result.Value }),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> Update(ISender sender, [FromBody] UpdateLookupDetailCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.Ok(),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> Delete(ISender sender, Guid id)
    {
        var result = await sender.Send(new DeleteLookupDetailCommand(id));

        return result.Match(
            onSuccess: Results.NoContent,
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> DeleteMultiple(ISender sender, [FromBody] Guid[] ids)
    {
        Result? result = null;
        foreach (var id in ids)
        {
             result = await sender.Send(new DeleteLookupDetailCommand(id));
        }
        return result!.Match(
            onSuccess: Results.NoContent,
            onFailure: result!.ToProblemDetails);
    }
}
