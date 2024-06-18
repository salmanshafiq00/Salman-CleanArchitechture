using CleanArchitechture.Domain.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitechture.Infrastructure.Persistence.Configurations;

internal sealed class AppMenuConfiguration : IEntityTypeConfiguration<AppMenu>
{
    public void Configure(EntityTypeBuilder<AppMenu> builder)
    {
        builder.Property(t => t.Label)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(t => t.Label).IsUnique();

        builder.Property(t => t.Url)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(500);
    }
}
