namespace CleanArchitechture.Application.Features.Common.Queries;

public record GetSelectListQuery<TId>(
    string Sql,
    object Parameters,
    string Key,
    bool? AllowCacheList = null)
    : ICacheableQuery<List<SelectListModel<TId>>>
{
    public TimeSpan? Expiration => null;
    public string CacheKey => Key;
    public bool? AllowCache => AllowCacheList ?? true;
}

internal sealed class GetSelectListQueryHandler<TId>(
    ISqlConnectionFactory sqlConnection)
    : IQueryHandler<GetSelectListQuery<TId>, List<SelectListModel<TId>>>
{
    public async Task<Result<List<SelectListModel<TId>>>> Handle(
        GetSelectListQuery<TId> request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var selectList = await connection
            .QueryAsync<SelectListModel<TId>>(request.Sql, request.Parameters);

        return Result.Success(selectList.AsList());
    }
}

