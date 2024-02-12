using CleanArchitechture.Application.Common.Interfaces;
using CleanArchitechture.Domain.Entities;
using CleanArchitechture.Domain.Events;

namespace CleanArchitechture.Application.TodoItems.Commands.CreateTodoItem;

public record CreateTodoItemCommand : IRequest<Guid>
{
    public Guid ListId { get; init; }

    public string? Title { get; init; }
}

public class CreateTodoItemCommandHandler : IRequestHandler<CreateTodoItemCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateTodoItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = new TodoItem
        {
            ListId = request.ListId,
            Title = request.Title,
            Done = false
        };

        entity.AddDomainEvent(new TodoItemCreatedEvent(entity));

        _context.TodoItems.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return Guid.NewGuid();
    }
}
