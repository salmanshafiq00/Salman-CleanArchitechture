using CleanArchitechture.Domain.Todos;

namespace CleanArchitechture.Domain.Todos.DomainEvents;

public class TodoItemCreatedEvent : BaseEvent
{
    public TodoItemCreatedEvent(TodoItem item)
    {
        Item = item;
    }

    public TodoItem Item { get; }
}
