namespace CleanArchitechture.Domain.Admin;

public class AppMenu : BaseAuditableEntity
{
    public Guid? ParentId { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Tooltip { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int OrderNo { get; set; }
    public bool Visible { get; set; }
    public string Description { get; set; } = string.Empty;

}
