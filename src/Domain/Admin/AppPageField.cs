namespace CleanArchitechture.Domain.Admin;

public class AppPageField
{
    public Guid Id { get; set; }
    public Guid AppPageId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public string DbField { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public string TextAlign { get; set; } = string.Empty;
    public bool IsSortable { get; set; } = true;
    public bool IsFilterable { get; set; } = false;
    public string DSName { get; set; } = string.Empty;
    public bool IsGlobalFilterable { get; set; } = false;
    public string FilterType { get; set; } = string.Empty;
    public bool? EnableLink { get; set; }
    public string LinkBaseUrl { get; set; } = string.Empty;
    public string LinkValueFieldName { get; set; } = string.Empty;
    public string BgColor { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public bool IsVisible { get; set; } = true;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual AppPage AppPage { get; set; } = default!;

}
