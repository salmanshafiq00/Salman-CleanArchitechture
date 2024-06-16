using System.Text.Json.Serialization;
using static CleanArchitechture.Application.Common.DapperQueries.SqlConstants;

namespace CleanArchitechture.Application.Features.Lookups.Queries;

[Authorize(Policy = Permissions.CommonSetup.Lookups.View)]
public record GetLookupByIdQuery(Guid Id) : ICacheableQuery<LookupModel?>
{
    [JsonIgnore]
    public string CacheKey => $"Lookup_{Id}";
    [JsonIgnore]
    public TimeSpan? Expiration => null;
    public bool? AllowCache => true;

}

internal sealed class GetLookupByIdQueryHandler(ISqlConnectionFactory sqlConnection)
    : IQueryHandler<GetLookupByIdQuery, LookupModel?>
{
    public async Task<Result<LookupModel?>> Handle(GetLookupByIdQuery query, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT 
                L.Id AS {nameof(LookupModel.Id)}, 
                L.Name AS {nameof(LookupModel.Name)}, 
                L.Code {nameof(LookupModel.Code)}, 
                L.ParentId AS {nameof(LookupModel.ParentId)}, 
                L.Description AS {nameof(LookupModel.Description)},
                L.Status AS {nameof(LookupModel.Status)},
                {S.CONV}(DATE, L.Created) AS {nameof(LookupModel.Created)}
            FROM dbo.Lookups AS l
            LEFT JOIN dbo.Lookups AS p ON p.Id = l.ParentId
            WHERE l.Id = @Id
            """;

        return await connection.QueryFirstOrDefaultAsync<LookupModel>(sql, new { query.Id });
    }
}
