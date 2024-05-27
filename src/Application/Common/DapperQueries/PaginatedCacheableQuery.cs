using System.Text.Json.Serialization;

namespace CleanArchitechture.Application.Common.DapperQueries;

public abstract record GridFeatureModel : IGridFeature
{
    public bool? AllowCache { get; set; } = true;

    // Pagination
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int Offset { get; set; } = 0;

    // Sortable
    public string SortField { get; set; } = string.Empty;
    public int? SortOrder { get; set; } = 1;

    // Filter

    public string GlobalFilterValue { get; set; } = string.Empty;
}


//[JsonIgnore]
//public abstract string CacheKey { get; }
//[JsonIgnore]
//public virtual TimeSpan? Expiration { get; set; } = null;

