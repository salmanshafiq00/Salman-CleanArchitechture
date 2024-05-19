namespace CleanArchitechture.Application.Features.Lookups.Queries;

[Authorize(Policy = Permissions.Lookups.View)]
public record GetLookupListQuery 
    : PaginatedFilter, ICacheableQuery<PaginatedResponse<LookupResponse>>
{
    public string CacheKey => $"Lookup_{PageNumber}_{PageSize}";

    public TimeSpan? Expiration => null;
}

internal sealed class GetLookupListQueryHandler(ISqlConnectionFactory sqlConnection) 
    : IQueryHandler<GetLookupListQuery, PaginatedResponse<LookupResponse>>
{
    public async Task<Result<PaginatedResponse<LookupResponse>>> Handle(GetLookupListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT 
                L.Id AS {nameof(LookupResponse.Id)}, 
                L.Name AS {nameof(LookupResponse.Name)}, 
                L.Code {nameof(LookupResponse.Code)}, 
                L.ParentId AS {nameof(LookupResponse.ParentId)}, 
                P.Name AS {nameof(LookupResponse.ParentName)} , 
                L.Description AS {nameof(LookupResponse.Description)},
                IIF(L.StatusName = 1, 'Active', 'Inactive') AS {nameof(LookupResponse.StatusName)}
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
