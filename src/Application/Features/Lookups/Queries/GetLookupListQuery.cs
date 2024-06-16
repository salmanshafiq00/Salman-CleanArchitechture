using System.Text.Json.Serialization;
using static CleanArchitechture.Application.Common.DapperQueries.SqlConstants;
using static CleanArchitechture.Application.Common.Security.Permissions;

namespace CleanArchitechture.Application.Features.Lookups.Queries;

[Authorize(Policy = CommonSetup.Lookups.View)]
public record GetLookupListQuery 
    : DataGridModel , ICacheableQuery<PaginatedResponse<LookupModel>>
{
    [JsonIgnore]
    public string CacheKey => $"Lookup_{Offset}_{PageSize}";
}

internal sealed class GetLookupListQueryHandler(ISqlConnectionFactory sqlConnection) 
    : IQueryHandler<GetLookupListQuery, PaginatedResponse<LookupModel>>
{
    public async Task<Result<PaginatedResponse<LookupModel>>> Handle(GetLookupListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT 
                L.Id AS {nameof(LookupModel.Id)}, 
                L.Name AS {nameof(LookupModel.Name)}, 
                L.Code AS {nameof(LookupModel.Code)}, 
                L.ParentId AS {nameof(LookupModel.ParentId)}, 
                P.Name AS {nameof(LookupModel.ParentName)} , 
                L.Description AS {nameof(LookupModel.Description)},
                IIF(L.Status = 1, 'Active', 'Inactive') AS {nameof(LookupModel.StatusName)},
                {S.CONV}(DATE, L.Created) AS {nameof(LookupModel.Created)}
            FROM dbo.Lookups AS L
            LEFT JOIN dbo.Lookups AS P ON P.Id = L.ParentId
            """;

        return await PaginatedResponse<LookupModel>
            //.CreateAsync(connection, sql, sqlWithOrders, request.Offset, request.PageSize);
            .CreateAsync(connection, sql, request, dataFields: LookupModel.DataFields);
            //.CreateAsync(connection, sql, request, dataFields: new List<DataFieldModel>(LookupModel.DataFields));
            
    }
}
