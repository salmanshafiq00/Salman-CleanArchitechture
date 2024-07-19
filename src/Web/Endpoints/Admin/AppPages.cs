using CleanArchitechture.Application.Common.DapperQueries;
using CleanArchitechture.Application.Features.Admin.AppPages.Commands;
using CleanArchitechture.Application.Features.Admin.AppPages.Queries;
using CleanArchitechture.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitechture.Web.Endpoints.Admin;

public class AppPages : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(GetAppPages)
            .MapGet(GetAppPage, "GetPage/{id}")
            .MapPost(UpsertPage, "UpsertPage");

    }

    [ProducesResponseType(typeof(PaginatedResponse<AppPageModel>), StatusCodes.Status200OK)]
    public async Task<IResult> GetAppPages(ISender sender, [FromBody] GetAppPageListQuery query)
    {
        var result = await sender.Send(query);

        return TypedResults.Ok(result.Value);
    }

    [ProducesResponseType(typeof(AppPageModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetAppPage(ISender sender, [FromRoute] Guid id)
    {
        var result = await sender.Send(new GetAppPageByIdQuery(id));

        return result.Match(
             onSuccess: () => Results.Ok(result.Value),
             onFailure: result.ToProblemDetails);
    }

    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> UpsertPage(ISender sender, [FromBody] UpsertAppPageCommand command)
    {
        var result = await sender.Send(command);

        return Results.Ok(result);
    }

    //[ProducesResponseType(StatusCodes.Status204NoContent)]
    //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    //public async Task<IResult> UpdatePage(ISender sender, [FromBody] UpdateAppPageCommand command)
    //{
    //    var result = await sender.Send(command);

    //    return result.Match(
    //         onSuccess: () => Results.NoContent(),
    //         onFailure: result.ToProblemDetails);
    //}

}
