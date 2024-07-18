namespace CleanArchitechture.Domain.Admin;

public class AppPageAction
{
    public Guid Id { get; set; }
    public Guid? ApplicationPageId { get; set; }
    public int? ActionTypeId { get; set; }
    public string ActionName { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;             
    public string Icon { get; set; } = string.Empty;
    public string FunctionName { get; set; } = string.Empty;
    public string NavigationUrl { get; set; } = string.Empty;
    public bool? ShowCaptionDevice { get; set; }
    public bool? ShowCaptionWeb { get; set; }
    public string Position { get; set; } = string.Empty;    
    public Guid? ParentId { get; set; }
    public int SortOrder { get; set; }
    public bool? IsActive { get; set; }
}
