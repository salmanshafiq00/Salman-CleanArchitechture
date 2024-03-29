﻿using CleanArchitechture.Application.Common.Abstractions;
using CleanArchitechture.Domain.Todos;

namespace CleanArchitechture.Application.TodoLists.Commands.CreateTodoList;

public record CreateTodoListCommand : IRequest<Guid>
{
    public string? Title { get; init; }
}

public class CreateTodoListCommandHandler : IRequestHandler<CreateTodoListCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateTodoListCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateTodoListCommand request, CancellationToken cancellationToken)
    {
        var entity = new TodoList();

        entity.Title = request.Title;

        _context.TodoLists.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return Guid.NewGuid();
    }
}
