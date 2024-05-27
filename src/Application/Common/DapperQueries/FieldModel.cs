namespace CleanArchitechture.Application.Common.DapperQueries;

public record class FieldModel
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool Visible { get; set; }
    public bool Sortable { get; set; }
    public int SortOrder { get; set; }
}
