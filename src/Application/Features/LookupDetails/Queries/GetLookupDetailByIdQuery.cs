using Dapper;

namespace CleanArchitechture.Application.Features.LookupDetails.Queries;

[Authorize(Policy = Permissions.LookupDetails.View)]
public record GetLookupDetailByIdQuery(Guid Id) : ICacheableQuery<LookupDetailResponse>
{
    public string CacheKey => $"LookupDetail_{Id}";

    public TimeSpan? Expiration => null;

}

internal sealed class GetLookupDetailByIdQueryHandler(ISqlConnectionFactory sqlConnection) 
    : IQueryHandler<GetLookupDetailByIdQuery, LookupDetailResponse>
{
    public async Task<LookupDetailResponse> Handle(GetLookupDetailByIdQuery query, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT 
                ld.Id, 
                ld.Name, 
                ld.Code, 
                ld.ParentId, 
                P.Name AS ParentName, 
                ld.Description,
                IIF(ld.Status = 1, 'Active', 'Inactive') AS Status,
                ld.LookupId,
                l.Name AS LookupName
            FROM dbo.LookupDetails AS ld
            INNER JOIN dbo.Lookups AS l ON l.Id = ld.LookupId
            LEFT JOIN dbo.LookupDetails AS p ON p.Id = ld.ParentId
            WHERE ld.Id = @Id
            """;

        return await connection.QueryFirstOrDefaultAsync<LookupDetailResponse>(sql, new { query.Id });
    }
}
