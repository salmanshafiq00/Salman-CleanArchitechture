using CleanArchitechture.Domain.Abstractions;

namespace CleanArchitechture.Domain.Common.DomainEvents;

public class LookupUpdatedEvent(Lookup lookup) : BaseEvent
{
    public Lookup Lookup { get; } = lookup;
}
