using System.Data;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitechture.Application.Common.DapperQueries;

public class PaginatedResponse<TEntity>
    where TEntity : class
{
    [System.Text.Json.Serialization.JsonInclude]
    public IReadOnlyCollection<TEntity> Items { get; init; }
    public int PageNumber { get; init; }
    public int TotalPages { get; init; }
    public int TotalCount { get; init; }

    public PaginatedResponse(){}

    public PaginatedResponse(IReadOnlyCollection<TEntity> items, int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Items = items;
    }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public static async Task<PaginatedResponse<TEntity>> CreateAsync(
     IDbConnection connection,
     string sql,
     GridFeatureModel gridModel,
     string? defaultOrderFieldName = null,
     object? parameters = default)
    {
        var logger = ServiceLocator.ServiceProvider
            .GetRequiredService<ILogger<PaginatedResponse<TEntity>>>();

        string defaultOrderBy = $"ORDER BY {defaultOrderFieldName ?? "(SELECT NULL)"}";

        string sqlOrderBy = GetOrderBySql(gridModel);

        if (string.IsNullOrEmpty(sqlOrderBy))
        {
            sqlOrderBy = defaultOrderBy;
        }

        var paginatedSql = $"""
            {sql}
            {sqlOrderBy} 
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        logger?.LogInformation("Executing SQL: {Sql}", paginatedSql);

        var items = await connection
            .QueryAsync<TEntity>(paginatedSql, new { gridModel.Offset, gridModel.PageSize, parameters });

        var count = await connection
            .ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM ({sql}) as CountQuery", parameters ?? new { });

        return new PaginatedResponse<TEntity>(items.AsList(), count, (gridModel.Offset / gridModel.PageSize) + 1, gridModel.PageSize);
    }

    private static string GetOrderBySql(GridFeatureModel gridModel)
    {
        if(string.IsNullOrEmpty(gridModel.SortField) || gridModel.SortField is null)
        {
            return string.Empty;
        }

        return gridModel.SortOrder == -1
            ? $"ORDER BY {gridModel.SortField} DESC"
            : $"ORDER BY {gridModel.SortField} ASC";
    }
}
