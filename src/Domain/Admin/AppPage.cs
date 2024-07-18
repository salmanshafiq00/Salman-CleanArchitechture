namespace CleanArchitechture.Domain.Admin;

public class AppPage : BaseAuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string SubTitle { get; set; } = string.Empty;
    public string RouterLink { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Permission { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string AppPageLayout { get; set; } = string.Empty;

    public virtual ICollection<AppPageField> AppPageFields { get; set; } = default!;
}
