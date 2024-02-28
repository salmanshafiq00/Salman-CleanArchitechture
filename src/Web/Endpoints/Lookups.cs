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

    public async Task<IResult> GetLookups(
        ISender sender, [AsParameters] GetLookupListQuery query)
    {
        var result = await sender.Send(query);
        return result.IsFailure
            ? result.ToProblemDetails()
            : TypedResults.Ok(result);
    }

    public async Task<IResult> GetLookup(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetLookupByIdQuery(id));
        return result.IsFailure
            ? result.ToProblemDetails()
            : TypedResults.Ok(result);
    }

    public async Task<IResult> CreateLookup(ISender sender, [FromBody] CreateLookupCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
             onSuccess: () => Results.Ok(),
             onFailure: result.ToProblemDetails);
    }

    //public async Task<IResult> UpdateLookup(ISender sender, [FromBody] UpdateLookupCommand command)
    //{
    //    await sender.Send(command);
    //    return Results.NoContent();
    //}

    public async Task<IResult> UpdateLookup(ISender sender, [FromBody] UpdateLookupCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
             onSuccess: () => Results.Ok(),
             onFailure: result.ToProblemDetails);
    }

    public async Task<IResult> DeleteLookup(ISender sender, Guid id)
    {
        var result = await sender.Send(new DeleteLookupCommand(id));

        return result.Match(
             onSuccess: () => Results.Ok(),
             onFailure: result.ToProblemDetails);
    }
}
