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

    public async Task<PaginatedResponse<LookupResponse>> GetLookups(ISender sender, [AsParameters] GetLookupsQuery query)
    {
        return await sender.Send(query);
    }

    public async Task<LookupResponse> GetLookup(ISender sender, Guid id)
    {
        return await sender.Send(new GetLookupByIdQuery(id));
    }

    public async Task<IResult> CreateLookup(ISender sender, [FromBody] CreateLookupCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
                onSucceed: () => Results.Ok(result),
                onFailed: result.ToProblemDetails);
    }

    //public async Task<IResult> UpdateLookup(ISender sender, [FromBody] UpdateLookupCommand command)
    //{
    //    await sender.Send(command);
    //    return Results.NoContent();
    //}

    public async Task<IResult> UpdateLookup(ISender sender, [FromBody] UpdateLookupCommand command)
    {
        var result =  await sender.Send(command);

        return result.Match(
            onSucceed: () => Results.Ok(result),
            onFailed: result.ToProblemDetails);
    }

    public async Task<IResult> DeleteLookup(ISender sender, Guid id)
    {
        var result = await sender.Send(new DeleteLookupCommand(id));

        return result.Match(
             onSucceed: () => Results.Ok(result),
             onFailed: result.ToProblemDetails);
    }
}
