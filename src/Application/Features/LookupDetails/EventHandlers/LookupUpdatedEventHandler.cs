using CleanArchitechture.Domain.Events;
using Dapper;

namespace CleanArchitechture.Application.Features.LookupDetails.EventHandlers;

internal sealed class LookupUpdatedEventHandler(ISqlConnectionFactory sqlConnection) 
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

       await connection.ExecuteAsync(sql, new { Status = notification.Lookup.Status, LookupId = notification.Lookup.Id });
    }
}
