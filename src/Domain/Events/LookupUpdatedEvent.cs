namespace CleanArchitechture.Domain.Events;

public class LookupUpdatedEvent(Lookup lookup) : BaseEvent
{
    public Lookup Lookup { get; } = lookup;
}
