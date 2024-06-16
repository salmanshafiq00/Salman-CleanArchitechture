using CleanArchitechture.Application.Common.DapperQueries;
using CleanArchitechture.Application.Features.LookupDetails.Commands;
using CleanArchitechture.Application.Features.LookupDetails.Queries;
using CleanArchitechture.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitechture.Web.Endpoints;

public class LookupDetails : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
           .RequireAuthorization()
           .MapPost(GetLookupDetails, "GetLookupDetails")
           .MapGet(GetLookupDetail, "{id:Guid}")
           .MapPost(CreateLookupDetail)
           .MapPut(UpdateLookupDetail)
           .MapDelete(DeleteLookupDetail, "{id:Guid}");
    }

    [ProducesResponseType(typeof(PaginatedResponse<LookupDetailModel>), StatusCodes.Status200OK)]
    public async Task<IResult> GetLookupDetails(ISender sender,  GetLookupDetailListQuery query)
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result.Value);
    }

    [ProducesResponseType(typeof(LookupDetailModel), StatusCodes.Status200OK)]
    public async Task<IResult> GetLookupDetail(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetLookupDetailByIdQuery(id));
        return TypedResults.Ok(result.Value);

    }

    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> CreateLookupDetail(ISender sender, [FromBody] CreateLookupDetailCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
             onSuccess: () => Results.Ok(result.Value),
             onFailure: result.ToProblemDetails);
    }


    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> UpdateLookupDetail(ISender sender, [FromBody] UpdateLookupDetailCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
             onSuccess: () => Results.Ok(),
             onFailure: result.ToProblemDetails);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> DeleteLookupDetail(ISender sender, Guid id)
    {      
        var result = await sender.Send(new DeleteLookupDetailCommand(id));

        return result.Match(
             onSuccess: Results.NoContent,
             onFailure: result.ToProblemDetails);
    }
}
