using CleanArchitechture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitechture.Infrastructure.Persistence.Configurations;

public class LookupConfiguration : IEntityTypeConfiguration<Lookup>
{
    public void Configure(EntityTypeBuilder<Lookup> builder)
    {
        builder.Property(t => t.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(t => t.Name).IsUnique();

        builder.Property(t => t.Code)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(1000);
    }
}
