using CleanArchitechture.Application.Features.Lookups.Queries;

namespace CleanArchitechture.Application.Features.LookupDetails.Queries;

[Authorize(Policy = Permissions.LookupDetails.View)]
public record GetLookupDetailByIdQuery(Guid Id) : ICacheableQuery<LookupDetailResponse>
{
    public string CacheKey => $"LookupDetail_{Id}";

    public TimeSpan? Expiration => null;

    public bool? AllowCache => throw new NotImplementedException();
}

internal sealed class GetLookupDetailByIdQueryHandler(ISqlConnectionFactory sqlConnection) 
    : IQueryHandler<GetLookupDetailByIdQuery, LookupDetailResponse>
{
    public async Task<Result<LookupDetailResponse>> Handle(GetLookupDetailByIdQuery query, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT 
                ld.Id AS {nameof(LookupDetailResponse.Id)}, 
                ld.Name AS {nameof(LookupDetailResponse.Name)}, 
                ld.Code AS {nameof(LookupDetailResponse.Code)}, 
                ld.ParentId  AS {nameof(LookupDetailResponse.ParentId)}, 
                ld.Description AS {nameof(LookupDetailResponse.Description)},
                ld.Status AS {nameof(LookupDetailResponse.Status)},
                ld.LookupId AS {nameof(LookupDetailResponse.LookupId)},
            FROM dbo.LookupDetails AS ld
            INNER JOIN dbo.Lookups AS l ON l.Id = ld.LookupId
            LEFT JOIN dbo.LookupDetails AS p ON p.Id = ld.ParentId
            WHERE ld.Id = @Id
            """;

        return await connection.QueryFirstOrDefaultAsync<LookupDetailResponse>(sql, new { query.Id });
    }
}
