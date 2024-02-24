namespace CleanArchitechture.Application.Features.Lookups.Queries;

[Authorize(Policy = Permissions.Lookups.View)]
public record GetLookupListQuery 
    : PaginatedFilter, ICacheableQuery<Result<PaginatedResponse<LookupResponse>>>
{
    public string CacheKey => $"Lookup_{PageNumber}_{PageSize}";

    public TimeSpan? Expiration => null;
}

internal sealed class GetLookupListQueryHandler(ISqlConnectionFactory sqlConnection) 
    : IQueryHandler<GetLookupListQuery, Result<PaginatedResponse<LookupResponse>>>
{
    public async Task<Result<PaginatedResponse<LookupResponse>>> Handle(GetLookupListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT 
                L.Id, 
                L.Name, 
                L.Code, 
                L.ParentId, 
                P.Name AS ParentName, 
                L.Description,
                IIF(L.Status = 1, 'Active', 'Inactive') AS Status
            FROM dbo.Lookups AS L
            LEFT JOIN dbo.Lookups AS P ON P.Id = L.ParentId
            """;

        var sqlWithOrders = $"""
            {sql} 
            ORDER BY L.Created
            """;

        return await PaginatedResponse<LookupResponse>
            .CreateAsync(connection, sql, sqlWithOrders, request.PageNumber, request.PageSize);
    }
}
