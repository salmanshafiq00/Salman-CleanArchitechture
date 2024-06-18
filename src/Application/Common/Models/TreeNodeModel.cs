namespace CleanArchitechture.Application.Common.Models;

public class TreeNodeModel<TKey>
{
    public TKey Key { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public TKey? ParentId { get; set; }
    public bool DisabledCheckbox { get; set; } = false;
    public bool Disabed { get; set; } = false;
    public bool Visible { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public bool partialSelected { get; set; }
    public bool Leaf { get { return Children.Count == 0; } }
    public IList<TreeNodeModel<TKey>> Children { get; set; } = [];
}

public class TreeNodeModel : TreeNodeModel<Guid>
{
}
