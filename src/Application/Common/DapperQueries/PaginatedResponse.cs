using System.Data;
using System.Text;
using CleanArchitechture.Application.Common.Extensions;
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
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    [System.Text.Json.Serialization.JsonInclude]

    public IReadOnlyCollection<DataFieldModel> DataFields { get; init; } = [];

    public PaginatedResponse() { }

    public PaginatedResponse(
        IReadOnlyCollection<TEntity> items,
        int count,
        int pageNumber,
        int pageSize,
        IReadOnlyCollection<DataFieldModel> dataFields)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Items = items;
        DataFields = dataFields;
    }

    public static async Task<PaginatedResponse<TEntity>> CreateAsync(
         IDbConnection connection,
         string sql,
         DataGridModel gridModel,
         IReadOnlyCollection<DataFieldModel> dataFields,
         object? parameters = default)
    {
        var logger = ServiceLocator.ServiceProvider
            .GetRequiredService<ILogger<PaginatedResponse<TEntity>>>();

        // WHERE

        SetWhereCluaseIfNotExist(ref sql);

        if (!string.IsNullOrEmpty(gridModel.GlobalFilterValue))
        {
            SetGlobalFilterSql(gridModel, ref sql, dataFields);
        }

        SetFilterSql(ref sql, gridModel);

        // ORDER BY

        string defaultOrderBy = $"ORDER BY {gridModel.DefaultOrderFieldName ?? "(SELECT NULL)"}";

        string sqlOrderBy = GetOrderBySql(gridModel);

        if (string.IsNullOrEmpty(sqlOrderBy))
        {
            sqlOrderBy = defaultOrderBy;
        }

        // Pagination

        var paginatedSql = $"""
            {sql}
            {sqlOrderBy} 
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        logger?.LogInformation("Executing SQL: {Sql}", paginatedSql);

        var param = new DynamicParameters(parameters);
        param.Add(nameof(gridModel.Offset), gridModel.Offset);
        param.Add(nameof(gridModel.PageSize), gridModel.PageSize);

        if (!string.IsNullOrEmpty(gridModel.GlobalFilterValue))
        {
            param.Add(nameof(gridModel.GlobalFilterValue), $"%{gridModel.GlobalFilterValue}%");
        }

        var items = await connection
            .QueryAsync<TEntity>(paginatedSql, new { gridModel.Offset, gridModel.PageSize, parameters });

        var count = await connection
            .ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM ({sql}) as CountQuery", parameters ?? new { });

        return new PaginatedResponse<TEntity>(
            items.AsList(),
            count,
            (gridModel.Offset / gridModel.PageSize) + 1,
            gridModel.PageSize,
            dataFields);
    }


    // WHERE functions

    private static bool HasWhereClause(string sql)
    {
        return sql.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase) >= 0;
    }


    private static void SetWhereCluaseIfNotExist(ref string sql)
    {
        if (HasWhereClause(sql))
        {
            return;
        }
        string defaultWhereSql = "WHERE 1 = 1";

        sql = $"""
            {sql} 
            {defaultWhereSql} 
            """;
    }

    private static void SetGlobalFilterSql(
        DataGridModel gridModel,
        ref string sql,
        IReadOnlyCollection<DataFieldModel> DataFields)
    {

        if (string.IsNullOrEmpty(gridModel.GlobalFilterValue))
        {
            return;
        }

        //if (globalSearchColumns is null)
        //{
        //    return;
        //}

        //var columnNameList = UtilityExtensions.GetColumnNameFromClass(typeof(TEntity)).ToHashSet();

        //if (globalSearchColumns is not null)
        //{
        //    foreach (var unMappedColumn in globalSearchColumns)
        //    {
        //        columnNameList.Add(unMappedColumn);
        //    }
        //}

        StringBuilder globalFilter = new();

        bool isFirst = true;
        foreach (var dataField in DataFields.Where(x => x.IsGlobalFilterable))
        {
            if (isFirst)
            {
                globalFilter.Append($" And LOWER({dataField.DbField}) LIKE '%{gridModel.GlobalFilterValue.ToLower()}%'");
                isFirst = false;
            }
            else
            {
                globalFilter.Append($" OR LOWER({dataField.DbField}) LIKE '%{gridModel.GlobalFilterValue.ToLower()}%'");
            }
        }

        sql = $"""
            {sql} 
            {globalFilter.ToString()}
            """;
    }

    private static void SetFilterSql(ref string sql, DataGridModel gridModel)
    {
        if (!gridModel.Filters.Any())
        {
            return;
        }

        StringBuilder filterBuilder = new();

        foreach (var filter in gridModel.Filters.Where(x => !string.IsNullOrEmpty(x.Value)))
        {
            filterBuilder.Append($" {filter.Operator} ");

            string condition = filter.MatchMode switch
            {
                "startsWith" => $"LOWER({filter.Field}) LIKE LOWER('{filter.Value}%')",
                "contains" => $"LOWER({filter.Field}) LIKE LOWER('%{filter.Value}%')",
                "notContains" => $"LOWER({filter.Field}) NOT LIKE LOWER('%{filter.Value}%')",
                "endsWith" => $"LOWER({filter.Field}) LIKE LOWER('%{filter.Value}')",
                "equals" => $"LOWER({filter.Field}) = LOWER('{filter.Value}')",
                "notEquals" => $"LOWER({filter.Field}) != LOWER('{filter.Value}')",
                _ => throw new InvalidOperationException($"Unknown MatchMode: {filter.MatchMode}")
            };

            filterBuilder.Append(condition);
        }

        sql = $"""
            {sql} 
            {filterBuilder.ToString()}
            """;
    }



    // ORDER BY

    private static string GetOrderBySql(DataGridModel gridModel)
    {
        if (string.IsNullOrEmpty(gridModel.SortField) || gridModel.SortField is null)
        {
            return string.Empty;
        }

        return gridModel.SortOrder == -1
            ? $"ORDER BY {gridModel.SortField} DESC"
            : $"ORDER BY {gridModel.SortField} ASC";
    }


    // Pagination



}
