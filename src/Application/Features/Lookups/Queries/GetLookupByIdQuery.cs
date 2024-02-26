using Dapper;

namespace CleanArchitechture.Application.Features.Lookups.Queries;

[Authorize(Policy = Permissions.Lookups.View)]
public record GetLookupByIdQuery(Guid Id) : ICacheableQuery<Result<LookupResponse>>
{
    public string CacheKey => $"Lookup_{Id}";

    public TimeSpan? Expiration => null;

}

internal sealed class GetLookupByIdQueryHandler(ISqlConnectionFactory sqlConnection) 
    : IQueryHandler<GetLookupByIdQuery, Result<LookupResponse>>
{
    public async Task<Result<LookupResponse>> Handle(GetLookupByIdQuery query, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT 
                l.Id, 
                l.Name, 
                l.Code, 
                l.ParentId, 
                p.Name AS ParentName, 
                l.Description,
                IIF(l.Status = 1, 'Active', 'Inactive') AS Status
            FROM dbo.Lookups AS l
            LEFT JOIN dbo.Lookups AS p ON p.Id = l.ParentId
            WHERE l.Id = @Id
            """;

        return await connection.QueryFirstOrDefaultAsync<LookupResponse>(sql, new { query.Id });
    }
}
