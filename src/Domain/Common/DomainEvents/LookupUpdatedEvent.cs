namespace CleanArchitechture.Domain.Common;

public class LookupUpdatedEvent(Lookup lookup) : BaseEvent
{
    public Lookup Lookup { get; } = lookup;
}
