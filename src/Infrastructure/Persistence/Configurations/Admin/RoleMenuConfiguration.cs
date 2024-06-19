using CleanArchitechture.Domain.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitechture.Infrastructure.Persistence.Configurations;

internal sealed class RoleMenuConfiguration : IEntityTypeConfiguration<RoleMenu>
{
    public void Configure(EntityTypeBuilder<RoleMenu> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(t => new {t.RoleId, t.AppMenuId})
            .IsUnique();

        builder.HasOne<AppMenu>()
            .WithMany()
            .HasForeignKey(x => x.AppMenuId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
