namespace CleanArchitechture.Domain.Entities;

public class LookupDetail : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Status { get; set; }
    public Guid? ParentId { get; set; }
    public Guid LookupId { get; set; }
    public int? DevCode { get; set; }


    public virtual LookupDetail Parent { get; set; } = default!;
    public virtual Lookup Lookup { get; set; } = default!;
}
