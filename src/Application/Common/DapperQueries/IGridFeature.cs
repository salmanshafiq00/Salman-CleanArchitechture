namespace CleanArchitechture.Application.Common.DapperQueries;

public interface IGridFeature : IPaginated, ISortable, IFilterable
{
    bool? AllowCache { get; set; }
}

public interface IPaginated
{
    int PageNumber { get; }
    int PageSize { get; set; }
    int Offset { get; set; }
}

public interface ISortable
{
    string SortField { get; set; }
    int? SortOrder { get; set; }
}

public interface IFilterable
{
    string GlobalFilterValue { get; set; }
}

