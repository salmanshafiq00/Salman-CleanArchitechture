using System.Text.Json.Serialization;
using CleanArchitechture.Application.Features.Lookups.Queries;

namespace CleanArchitechture.Application.Features.LookupDetails.Queries;

[Authorize(Policy = Permissions.LookupDetails.View)]
public record GetLookupDetailListQuery
    : DataGridModel, ICacheableQuery<PaginatedResponse<LookupDetailResponse>>
{
    [JsonInclude]
    public string CacheKey => $"LookupDetail_{PageNumber}_{PageSize}";
}

internal sealed class GetLookupDetailListQueryHandler(ISqlConnectionFactory sqlConnection) 
    : IQueryHandler<GetLookupDetailListQuery, PaginatedResponse<LookupDetailResponse>>
{
    public async Task<Result<PaginatedResponse<LookupDetailResponse>>> Handle(GetLookupDetailListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT 
                ld.Id  AS {nameof(LookupDetailResponse.Id)}, 
                ld.Name AS {nameof(LookupDetailResponse.Name)}, 
                ld.Code AS {nameof(LookupDetailResponse.Code)}, 
                ld.ParentId AS {nameof(LookupDetailResponse.ParentId)}, 
                p.Name AS {nameof(LookupDetailResponse.ParentName)}, 
                ld.Description AS {nameof(LookupDetailResponse.Description)},
                IIF(ld.Status = 1, 'Active', 'Inactive') AS {nameof(LookupDetailResponse.StatusName)},
                ld.LookupId AS {nameof(LookupDetailResponse.LookupId)}, 
                l.Name AS {nameof(LookupDetailResponse.LookupName)}
            FROM dbo.LookupDetails AS ld
            INNER JOIN dbo.Lookups l ON l.Id = ld.LookupId
            LEFT JOIN dbo.LookupDetails AS p ON p.Id = ld.ParentId
            """;

        var sqlWithOrders = $"""
            {sql} 
            ORDER BY ld.Created
            """;

        return await PaginatedResponse<LookupDetailResponse>
            //.CreateAsync(connection, sql, sqlWithOrders, request.PageNumber, request.PageSize);
            .CreateAsync(connection, sql, request, dataFields: LookupResponse.DataFields);
    }
}
