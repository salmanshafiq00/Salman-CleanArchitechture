using System.Text.Json.Serialization;
using static CleanArchitechture.Application.Common.DapperQueries.SqlConstants;

namespace CleanArchitechture.Application.Features.Admin.AppPages.Queries;

//[Authorize(Policy = Permissions.Admin.AppPages.View)]
public record GetAppPageListQuery
    : DataGridModel , ICacheableQuery<PaginatedResponse<AppPageModel>>
{
    [JsonIgnore]
    public string CacheKey => $"AppPage_{Offset}_{PageSize}";
}

internal sealed class GetAppPageListQueryHandler(ISqlConnectionFactory sqlConnection) 
    : IQueryHandler<GetAppPageListQuery, PaginatedResponse<AppPageModel>>
{
    public async Task<Result<PaginatedResponse<AppPageModel>>> Handle(GetAppPageListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT 
                ap.Id AS {nameof(AppPageModel.Id)}, 
                ap.Name AS {nameof(AppPageModel.Name)}, 
                ap.RouterLink AS {nameof(AppPageModel.RouterLink)}, 
                ap.Title AS {nameof(AppPageModel.Title)}, 
                IIF(ap.IsActive = 1, 'Active', 'Inactive') AS {nameof(AppPageModel.Active)}
            FROM dbo.AppPages AS ap
            """;

        return await PaginatedResponse<AppPageModel>
            .CreateAsync(connection, sql, request, dataFields: AppPageModel.DataFields);
            
    }
}
