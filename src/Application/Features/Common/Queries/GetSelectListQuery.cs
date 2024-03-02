using CleanArchitechture.Application.Common.Abstractions;
using CleanArchitechture.Application.Common.Abstractions.Messaging;

namespace CleanArchitechture.Application.Features.Common.Queries;

public record GetSelectListQuery(string Sql, object Parameters, string Key) 
    : ICacheableQuery<List<SelectListModel>>
{
    public TimeSpan? Expiration => null;

    public string CacheKey => Key;
}

internal sealed class GetSelectListQueryHandler(
    ISqlConnectionFactory sqlConnection)
    : IQueryHandler<GetSelectListQuery, List<SelectListModel>>
{
    public async Task<Result<List<SelectListModel>>> Handle(GetSelectListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var selectList = await connection
            .QueryAsync<SelectListModel>(request.Sql, request.Parameters);

        return Result.Success(selectList.AsList());
    }
}
