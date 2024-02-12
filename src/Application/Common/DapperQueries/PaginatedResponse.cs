using System.Data;
using Dapper;

namespace CleanArchitechture.Application.Common.DapperQueries;

public class PaginatedResponse<TEntity>
    where TEntity : class
{
    [System.Text.Json.Serialization.JsonInclude]
    public IReadOnlyCollection<TEntity> Items { get; init; }
    public int PageNumber { get; init; }
    public int TotalPages { get; init; }
    public int TotalCount { get; init; }

    public PaginatedResponse()
    {
        
    }

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
        string sqlWithOrders, 
        int pageNumber, 
        int pageSize,
        object? parameters = default)
    {
        var count = await connection
            .ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM ({sql}) as CountQuery", parameters ?? new { });

        sql = $"""
            {sqlWithOrders} 
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        var items = await connection
            .QueryAsync<TEntity>(sql, new { Offset = (pageNumber - 1) * pageSize, PageSize = pageSize, parameters });

        return new PaginatedResponse<TEntity>(items.AsList(), count, pageNumber, pageSize);
    }

}
