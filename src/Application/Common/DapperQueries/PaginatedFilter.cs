namespace CleanArchitechture.Application.Common.DapperQueries;

public record PaginatedFilter
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Keyword { get; set; }
}
