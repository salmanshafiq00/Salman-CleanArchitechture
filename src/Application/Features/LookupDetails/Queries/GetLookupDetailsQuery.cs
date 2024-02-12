namespace CleanArchitechture.Application.Features.LookupDetails.Queries;

[Authorize(Policy = Permissions.LookupDetails.View)]
public record GetLookupDetailsQuery : ICacheableQuery<PaginatedResponse<LookupDetailResponse>>, IDapperPaginatedQuery
{
    public string CacheKey => $"LookupDetail_{PageNumber}_{PageSize}";

    public TimeSpan? Expiration => null;

    public int? PageNumber => 1;

    public int? PageSize => 10;
}

internal sealed class GetLookupDetailListQueryHandler(ISqlConnectionFactory sqlConnection) 
    : IQueryHandler<GetLookupDetailsQuery, PaginatedResponse<LookupDetailResponse>>
{
    public async Task<PaginatedResponse<LookupDetailResponse>> Handle(GetLookupDetailsQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT 
                ld.Id, 
                ld.Name, 
                ld.Code, 
                ld.ParentId, 
                p.Name AS ParentName, 
                ld.Description,
                IIF(ld.Status = 1, 'Active', 'Inactive') AS Status,
                ld.LookupId, 
                l.Name AS LookupName
            FROM dbo.LookupDetails AS ld
            INNER JOIN dbo.Lookups l ON l.Id = ld.LookupId
            LEFT JOIN dbo.LookupDetails AS p ON p.Id = ld.ParentId
            """;

        var sqlWithOrders = $"""
            {sql} 
            ORDER BY ld.Created
            """;

        return await PaginatedResponse<LookupDetailResponse>.CreateAsync(connection, sql, sqlWithOrders, request.PageNumber.Value, request.PageSize.Value);
    }
}
