namespace CleanArchitechture.Domain.Common;

public class Lookup : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Status { get; set; }
    public int? DevCode { get; set; }
    public Guid? ParentId { get; set; }

    public virtual Lookup Parent { get; set; } = default!;
}
