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
           .MapGet(GetLookupDetails)
           .MapGet(GetLookupDetail, "{id:Guid}")
           .MapPost(CreateLookupDetail)
           .MapPut(UpdateLookupDetail)
           .MapDelete(DeleteLookupDetail, "{id:Guid}");
    }

    public async Task<PaginatedResponse<LookupDetailResponse>> GetLookupDetails(ISender sender, [AsParameters] GetLookupDetailListQuery query)
    {
        return await sender.Send(query);
    }

    public async Task<LookupDetailResponse> GetLookupDetail(ISender sender, Guid id)
    {
        return await sender.Send(new GetLookupDetailByIdQuery(id));
    }

    public async Task<IResult> CreateLookupDetail(ISender sender, [FromBody] CreateLookupDetailCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
             onSuccess: () => Results.Ok(),
             onFailure: result.ToProblemDetails);
    }

    public async Task<IResult> UpdateLookupDetail(ISender sender, [FromBody] UpdateLookupDetailCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
             onSuccess: () => Results.Ok(),
             onFailure: result.ToProblemDetails);
    }

    public async Task<IResult> DeleteLookupDetail(ISender sender, Guid id)
    {      
        var result = await sender.Send(new DeleteLookupDetailCommand(id));

        return result.Match(
             onSuccess: () => Results.Ok(),
             onFailure: result.ToProblemDetails);
    }
}
