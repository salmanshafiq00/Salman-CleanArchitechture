using System.Text.Json.Serialization;

namespace CleanArchitechture.Application.Common.DapperQueries;

public abstract record DataGridModel : IDataGrid
{
    // Caching
    public bool? AllowCache { get; set; } = true;

    // Pagination
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int Offset { get; set; } = 0;

    // Sortable
    public string SortField { get; set; } = string.Empty;
    public int? SortOrder { get; set; } = 1;
    public string? DefaultOrderFieldName { get; set; } = null;


    //Global Filter
    public string GlobalFilterValue { get; set; } = string.Empty;

    // Filter
    public List<DataFilterModel> Filters { get; set; } = [];
}

public class DataFieldModel
{
    public string Field { get; set; } = string.Empty;
    public string Header { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    [JsonIgnore]
    public string DbField { get; set; } = string.Empty;
    public bool Visible { get; set; }
    public int SortOrder { get; set; }
    public bool IsSortable { get; set; }
    public bool IsGlobalFilterable { get; set; }
    public bool IsFilterable { get; set; }
    public string DSName { get; set; } = string.Empty;
}

public class DataFilterModel
{
    public string Field { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string MatchMode { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;
    public string DSName {  get; set; } = string.Empty;
    public List<SelectListModel> DataSource { get; set; } = [];
}


//[JsonIgnore]
//public abstract string CacheKey { get; }
//[JsonIgnore]
//public virtual TimeSpan? Expiration { get; set; } = null;

