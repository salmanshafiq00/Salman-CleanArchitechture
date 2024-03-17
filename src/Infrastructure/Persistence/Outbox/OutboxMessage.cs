using Newtonsoft.Json;

namespace CleanArchitechture.Infrastructure.Persistence.Outbox;

internal sealed record OutboxMessage(
    Guid Id,
    string Type,
    string Content,
    DateTime CreatedOn
)
{
    public DateTime? ProcessedOn { get; set; } = null;
    public string? Error { get; set; } = null;
}
