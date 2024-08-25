﻿using CleanArchitechture.Domain.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitechture.Infrastructure.Persistence.Configurations;

internal sealed class AppNotificationConfiguration : IEntityTypeConfiguration<AppNotification>
{
    public void Configure(EntityTypeBuilder<AppNotification> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(t => t.Title)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.SenderId)
            .HasMaxLength(100);

        builder.Property(t => t.RecieverId)
            .HasMaxLength(100);
    }
}
