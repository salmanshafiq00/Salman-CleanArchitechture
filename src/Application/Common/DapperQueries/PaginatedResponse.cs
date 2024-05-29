using System.Data;
using System.Text;
using CleanArchitechture.Application.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using static CleanArchitechture.Application.Common.DapperQueries.Constants;

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

        #region Filter (WHERE)

        SetWhereCluaseIfNotExist(ref sql);

        SetGlobalFilterSql(gridModel, ref sql, dataFields);

        SetFilterSql(ref sql, gridModel, dataFields);

        #endregion


        #region Sorting (ORDER BY)

        string defaultOrderBy = $"ORDER BY {gridModel.DefaultOrderFieldName ?? "(SELECT NULL)"}";

        string sqlOrderBy = GetOrderBySql(gridModel);

        if (string.IsNullOrEmpty(sqlOrderBy))
        {
            sqlOrderBy = defaultOrderBy;
        }

        #endregion

        #region Pagination  (FETCH NEXT)

        var paginatedSql = $"""
            {sql}
            {sqlOrderBy} 
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;


        #endregion

        logger?.LogInformation("Executing SQL: {Sql}", paginatedSql);

        #region Parameters

        var param = new DynamicParameters(parameters);
        param.Add(nameof(gridModel.Offset), gridModel.Offset);
        param.Add(nameof(gridModel.PageSize), gridModel.PageSize);

        if (!string.IsNullOrEmpty(gridModel.GlobalFilterValue))
        {
            param.Add(nameof(gridModel.GlobalFilterValue), $"%{gridModel.GlobalFilterValue.ToLower()}%");
        }

        #endregion


        var items = await connection
            .QueryAsync<TEntity>(paginatedSql, param);

        var count = await connection
            .ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM ({sql}) as CountQuery", param);

        return new PaginatedResponse<TEntity>(
            items.AsList(),
            count,
            (gridModel.Offset / gridModel.PageSize) + 1,
            gridModel.PageSize,
            dataFields);
    }


    #region Filtering Functions (WHERE)

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
        IReadOnlyCollection<DataFieldModel> dataFields)
    {

        if (string.IsNullOrEmpty(gridModel.GlobalFilterValue)
               || string.IsNullOrWhiteSpace(gridModel.GlobalFilterValue))
        {
            return;
        }

        StringBuilder globalFilter = new();

        bool isFirst = true;
        foreach (var dataField in dataFields.Where(x => x.IsGlobalFilterable))
        {
            if (isFirst)
            {
                globalFilter.Append($"(LOWER({dataField.DbField}) LIKE @GlobalFilterValue");
                isFirst = false;
            }
            else
            {
                globalFilter.Append($" OR LOWER({dataField.DbField}) LIKE @GlobalFilterValue");
            }
        }

        if (!isFirst) globalFilter.Append(")");

        sql = $"""
            {sql} 
            AND {globalFilter.ToString()}
            """;
    }

    private static void SetFilterSql(
        ref string sql, 
        DataGridModel gridModel,
        IReadOnlyCollection<DataFieldModel> dataFields)
    {
        if (!gridModel.Filters.Any() 
            || !gridModel.Filters.Any(x => string.IsNullOrEmpty(x.Value)))
        {
            return;
        }

        StringBuilder filterBuilder = new();

        foreach (var filter in gridModel.Filters.Where(x => !string.IsNullOrEmpty(x.Value)))
        {
            string sqlOperator = string.IsNullOrEmpty(filter.Operator) ? "AND" : filter.Operator.ToUpper();

            filterBuilder.Append(sqlOperator);

            var dataField = GetDataField(dataFields, filter.Field);

            if (dataField is null) continue;

            string condition = string.Empty;

            if (dataField?.DbDataType == FieldDataType.NVARCHAR) 
            {
                condition = filter.MatchMode switch
                {
                    "startsWith" => $"LOWER({filter.Field}) LIKE LOWER('{filter.Value}%')",
                    "contains" => $"LOWER({filter.Field}) LIKE LOWER('%{filter.Value}%')",
                    "notContains" => $"LOWER({filter.Field}) NOT LIKE LOWER('%{filter.Value}%')",
                    "endsWith" => $"LOWER({filter.Field}) LIKE LOWER('%{filter.Value}')",
                    "equals" => $"LOWER({filter.Field}) = LOWER('{filter.Value}')",
                    "notEquals" => $"LOWER({filter.Field}) != LOWER('{filter.Value}')",
                    _ => throw new InvalidOperationException($"Unknown MatchMode: {filter.MatchMode}")
                };
            }
            else if (dataField?.DbDataType == FieldDataType.BIT)
            {
                condition = filter.MatchMode switch
                {
                    "equals" => $"{filter.Field} = {filter.Value}",
                    "notEquals" => $"{filter.Field} != {filter.Value}",
                    _ => throw new InvalidOperationException($"Unknown MatchMode: {filter.MatchMode}")
                };

            }

            filterBuilder.Append(" ");
            filterBuilder.Append(condition);
        }

        sql = $"""
            {sql} 
            {filterBuilder.ToString()}
            """;
    }



    #endregion


    #region Sorting Functions (ORDER BY)

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


    #endregion

    private static string GetCondition(DataFilterModel filter)
    {

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

        return condition;
    }

    private static DataFieldModel? GetDataField(
        IReadOnlyCollection<DataFieldModel> dataFields, 
        string field)
    {
        return dataFields.FirstOrDefault(x => string.Equals(x.DbField, field));
    }
}
