using System.Text.Json.Serialization;
using CleanArchitechture.Application.Features.Lookups.Queries;

namespace CleanArchitechture.Application.Features.LookupDetails.Queries;

[Authorize(Policy = Permissions.CommonSetup.LookupDetails.View)]
public record GetLookupDetailByIdQuery(Guid Id) : ICacheableQuery<LookupDetailModel>
{
    [JsonIgnore]
    public string CacheKey => $"LookupDetail_{Id}";
    [JsonIgnore]
    public TimeSpan? Expiration => null;
    public bool? AllowCache => true;
}

internal sealed class GetLookupDetailByIdQueryHandler(ISqlConnectionFactory sqlConnection) 
    : IQueryHandler<GetLookupDetailByIdQuery, LookupDetailModel>
{
    public async Task<Result<LookupDetailModel>> Handle(GetLookupDetailByIdQuery query, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT 
                ld.Id AS {nameof(LookupDetailModel.Id)}, 
                ld.Name AS {nameof(LookupDetailModel.Name)}, 
                ld.Code AS {nameof(LookupDetailModel.Code)}, 
                ld.ParentId  AS {nameof(LookupDetailModel.ParentId)}, 
                ld.Description AS {nameof(LookupDetailModel.Description)},
                ld.Status AS {nameof(LookupDetailModel.Status)},
                ld.LookupId AS {nameof(LookupDetailModel.LookupId)},
            FROM dbo.LookupDetails AS ld
            INNER JOIN dbo.Lookups AS l ON l.Id = ld.LookupId
            LEFT JOIN dbo.LookupDetails AS p ON p.Id = ld.ParentId
            WHERE ld.Id = @Id
            """;

        return await connection.QueryFirstOrDefaultAsync<LookupDetailModel>(sql, new { query.Id });
    }
}
