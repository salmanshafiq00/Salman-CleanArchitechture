using CleanArchitechture.Application.Features.Admin.AppPages.Commands;
using CleanArchitechture.Application.Features.Admin.AppPages.Queries;

namespace CleanArchitechture.Web.Endpoints.Admin;

public class AppPages : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(GetAppPages)
            .MapGet(GetAppPage, "GetPage/{id}")
            .MapPost(CreateAppPage, "CreateAppPage")
            .MapPut(UpdateAppPage, "UpdateAppPage")
            .MapPost(UpsertAppPage, "UpsertAppPage");
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
    public async Task<IResult> CreateAppPage(ISender sender, [FromBody] CreateAppPageCommand command)
    {
        var result = await sender.Send(command);

        return Results.Ok(result.Value);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> UpdateAppPage(ISender sender, [FromBody] UpdateAppPageCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
             onSuccess: () => Results.NoContent(),
             onFailure: result.ToProblemDetails);
    }

    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> UpsertAppPage(ISender sender, [FromBody] UpsertAppPageCommand command)
    {
        var result = await sender.Send(command);

        return Results.Ok(result);
    }

}
