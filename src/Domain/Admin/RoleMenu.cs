namespace CleanArchitechture.Domain.Admin;

public class RoleMenu: BaseEntity
{
    public string RoleId { get; set; } = string.Empty;
    public Guid AppMenuId { get; set; }
}
