using CleanArchitechture.Application.Features.Common.Queries;
using CleanArchitechture.Application.Features.LookupDetails.Commands;
using CleanArchitechture.Application.Features.LookupDetails.Queries;

namespace CleanArchitechture.Web.Endpoints;

public class LookupDetails : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
           .RequireAuthorization()
           .MapPost(GetAll, "GetAll", "GetLookupDetails")
           .MapGet(Get, "Get/{id:Guid}", "GetLookupDetail")
           .MapPost(Create, "Create", "CreateLookupDetail")
           .MapPut(Update, "Update", "UpdateLookupDetail")
           .MapDelete(Delete, "Delete/{id:Guid}", "DeleteLookupDetail");
    }

    [ProducesResponseType(typeof(PaginatedResponse<LookupDetailModel>), StatusCodes.Status200OK)]
    public async Task<IResult> GetAll(ISender sender,  GetLookupDetailListQuery query)
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result.Value);
    }

    [ProducesResponseType(typeof(LookupDetailModel), StatusCodes.Status200OK)]
    public async Task<IResult> Get(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetLookupDetailByIdQuery(id));

        var lookupSelectList = await sender.Send(new GetSelectListQuery(
                Sql: SelectListSqls.GetLookupSelectListSql,
                Parameters: new { },
                Key: CacheKeys.Lookup_All_SelectList,
                AllowCacheList: false)
            );
        result.Value.OptionDataSources.Add("lookupSelectList", lookupSelectList.Value);

        var parentSelectList = await sender.Send(new GetSelectListQuery(
                Sql: SelectListSqls.GetLookupDetailSelectListSql,
                Parameters: new { },
                Key: CacheKeys.LookupDetail_All_SelectList,
                AllowCacheList: false)
            );
        result.Value.OptionDataSources.Add("parentSelectList", parentSelectList.Value);

        return TypedResults.Ok(result.Value);

    }

    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Create(ISender sender, [FromBody] CreateLookupDetailCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
             //onSuccess: () => Results.Ok(result.Value),
             onSuccess: () => Results.CreatedAtRoute("GetLookupDetail", new {id = result.Value}),
             onFailure: result.ToProblemDetails);
    }


    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> Update(ISender sender, [FromBody] UpdateLookupDetailCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
             onSuccess: () => Results.Ok(),
             onFailure: result.ToProblemDetails);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Delete(ISender sender, Guid id)
    {      
        var result = await sender.Send(new DeleteLookupDetailCommand(id));

        return result.Match(
             onSuccess: Results.NoContent,
             onFailure: result.ToProblemDetails);
    }
}
