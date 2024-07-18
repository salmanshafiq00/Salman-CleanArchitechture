using System.Text.Json.Serialization;
using CleanArchitechture.Application.Common.Extensions;

namespace CleanArchitechture.Application.Features.Admin.AppPages.Queries;
//[Authorize(Policy = Permissions.Admin.AppPages.View)]
public record GetAppPageByIdQuery(Guid Id) : ICacheableQuery<AppPageModel?>
{
    [JsonIgnore]
    public string CacheKey => $"AppPage_{Id}";
    [JsonIgnore]
    public TimeSpan? Expiration => null;
    public bool? AllowCache => false;

}

internal sealed class GetAppPageByIdQueryHandler(ISqlConnectionFactory sqlConnection)
    : IQueryHandler<GetAppPageByIdQuery, AppPageModel?>
{
    public async Task<Result<AppPageModel?>> Handle(GetAppPageByIdQuery query, CancellationToken cancellationToken)
    {
        if (query.Id.IsNullOrEmpty())
        {
            return new AppPageModel();
        }

        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT 
                ap.Id AS {nameof(AppPageModel.Id)}, 
                ap.Title AS {nameof(AppPageModel.Title)}, 
                ap.SubTitle AS {nameof(AppPageModel.SubTitle)}, 
                ap.RouterLink AS {nameof(AppPageModel.RouterLink)}, 
                ap.Name AS {nameof(AppPageModel.Name)}, 
                ap.Permission AS {nameof(AppPageModel.Permission)}, 
                ap.IsActive AS {nameof(AppPageModel.IsActive)}, 
                ap.AppPageLayout AS {nameof(AppPageModel.AppPageLayout)}
            FROM dbo.AppPages AS ap
            WHERE ap.Id = @Id
            """;

        return await connection.QueryFirstOrDefaultAsync<AppPageModel>(sql, new { query.Id });
    }
}
