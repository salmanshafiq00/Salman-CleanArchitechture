using CleanArchitechture.Domain.Admin;
using CleanArchitechture.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitechture.Infrastructure.Persistence.Configurations;

internal sealed class AppMenuConfiguration : IEntityTypeConfiguration<AppMenu>
{
    public void Configure(EntityTypeBuilder<AppMenu> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(t => t.Label)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(t => t.Label).IsUnique();

        builder.Property(t => t.Url)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(t => t.Tooltip)
            .HasMaxLength(100);

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.HasOne<LookupDetail>()
            .WithMany()
            .HasForeignKey(x => x.MenuTypeId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
