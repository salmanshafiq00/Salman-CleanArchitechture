using CleanArchitechture.Application.Common.Abstractions;
using CleanArchitechture.Application.Common.Events;
using CleanArchitechture.Domain.Common.DomainEvents;
using Dapper;

namespace CleanArchitechture.Application.Features.LookupDetails.EventHandlers;

internal sealed class LookupUpdatedEventHandler(ISqlConnectionFactory sqlConnection, IPublisher publisher)
    : INotificationHandler<LookupUpdatedEvent>
{
    public async Task Handle(LookupUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = """
            UPDATE dbo.LookupDetails
            SET Status = @Status
            WHERE LookupId = @LookupId
            """;

        var result = await connection.ExecuteAsync(sql, new { Status = notification.Lookup.Status, LookupId = notification.Lookup.Id });

        if (result > 0)
        {
            await publisher.Publish(
    new CacheInvalidationEvent { CacheKey = CacheKeys.LookupDetail });

        }
    }
}
