using CleanArchitechture.Application.Common.DapperQueries;
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
           .RequireAuthorization()
           .MapGet(GetLookups)
           .MapGet(GetLookup, "{id:Guid}")
           .MapPost(CreateLookup)
           .MapPut(UpdateLookup)
           .MapDelete(DeleteLookup, "{id:Guid}");
    }

    [ProducesResponseType(typeof(PaginatedResponse<LookupResponse>), StatusCodes.Status200OK)]
    public async Task<IResult> GetLookups(ISender sender, [AsParameters] GetLookupListQuery query)
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result.Value);
    }

    [ProducesResponseType(typeof(LookupResponse), StatusCodes.Status200OK)]
    public async Task<IResult> GetLookup(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetLookupByIdQuery(id));
        return TypedResults.Ok(result.Value);

    }

    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> CreateLookup(ISender sender, [FromBody] CreateLookupCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
             onSuccess: () => Results.Ok(),
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
    public async Task<IResult> DeleteLookup(ISender sender, Guid id)
    {
        var result = await sender.Send(new DeleteLookupCommand(id));

        return result.Match(
             onSuccess: Results.NoContent,
             onFailure: result.ToProblemDetails);
    }
}
