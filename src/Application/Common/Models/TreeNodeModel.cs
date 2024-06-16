namespace CleanArchitechture.Application.Common.Models;

public class TreeNodeModel<TKey>
{
    public TKey Key { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public TKey? ParentId { get; set; }
    public bool DisabledCheckbox { get; set; } = false;
    public bool Disabed { get; set; } = false;
    public bool IsActive { get; set; } 
    public IList<TreeNodeModel<TKey>> Children { get; set; } = [];
}

public class TreeNodeModel : TreeNodeModel<Guid>
{
}
