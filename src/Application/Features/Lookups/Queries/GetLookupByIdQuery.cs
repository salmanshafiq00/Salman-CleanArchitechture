namespace CleanArchitechture.Application.Features.Lookups.Queries;

[Authorize(Policy = Permissions.Lookups.View)]
public record GetLookupByIdQuery(Guid Id) : ICacheableQuery<LookupResponse?>
{
    public string CacheKey => $"Lookup_{Id}";

    public TimeSpan? Expiration => null;

}

internal sealed class GetLookupByIdQueryHandler(ISqlConnectionFactory sqlConnection) 
    : IQueryHandler<GetLookupByIdQuery, LookupResponse?>
{
    public async Task<Result<LookupResponse?>> Handle(GetLookupByIdQuery query, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT 
                L.Id AS {nameof(LookupResponse.Id)}, 
                L.Name AS {nameof(LookupResponse.Name)}, 
                L.Code {nameof(LookupResponse.Code)}, 
                L.ParentId AS {nameof(LookupResponse.ParentId)}, 
                L.Description AS {nameof(LookupResponse.Description)},
                L.StatusName AS {nameof(LookupResponse.Status)}
            FROM dbo.Lookups AS l
            LEFT JOIN dbo.Lookups AS p ON p.Id = l.ParentId
            WHERE l.Id = @Id
            """;

        return await connection.QueryFirstOrDefaultAsync<LookupResponse>(sql, new { query.Id });
    }
}
