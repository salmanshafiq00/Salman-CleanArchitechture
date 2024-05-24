using CleanArchitechture.Application.Common.Models;
using CleanArchitechture.Application.TodoItems.Commands.CreateTodoItem;
using CleanArchitechture.Application.TodoItems.Commands.DeleteTodoItem;
using CleanArchitechture.Application.TodoItems.Commands.UpdateTodoItem;
using CleanArchitechture.Application.TodoItems.Commands.UpdateTodoItemDetail;
using CleanArchitechture.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using CleanArchitechture.Web.Extensions;

namespace CleanArchitechture.Web.Endpoints;

public class TodoItems : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetTodoItemsWithPagination)
            .MapPost(CreateTodoItem)
            .MapPut(UpdateTodoItem, "{id}")
            .MapPut(UpdateTodoItemDetail, "UpdateDetail/{id}")
            .MapDelete(DeleteTodoItem, "{id}");
    }

    public async Task<PaginatedList<TodoItemBriefDto>> GetTodoItemsWithPagination(ISender sender, [AsParameters] GetTodoItemsWithPaginationQuery query)
    {
        var result = await sender.Send(query);
        return result.Value;
    }

    public async Task<Guid> CreateTodoItem(ISender sender, CreateTodoItemCommand command)
    {
        return await sender.Send(command);
    }

    public async Task<IResult> UpdateTodoItem(ISender sender, Guid id, UpdateTodoItemCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();
    }

    public async Task<IResult> UpdateTodoItemDetail(ISender sender, Guid id, UpdateTodoItemDetailCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();
    }

    public async Task<IResult> DeleteTodoItem(ISender sender, Guid id)
    {
        await sender.Send(new DeleteTodoItemCommand(id));
        return Results.NoContent();
    }
}
