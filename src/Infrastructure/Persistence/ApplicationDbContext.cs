﻿using System.Reflection;
using CleanArchitechture.Application.Common.Abstractions;
using CleanArchitechture.Domain.Admin;
using CleanArchitechture.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitechture.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    //public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    #region Admin

    public DbSet<AppMenu> AppMenus => Set<AppMenu>();
    public DbSet<RoleMenu> RoleMenus => Set<RoleMenu>();
    public DbSet<AppPage> AppPages => Set<AppPage>();
    public DbSet<AppNotification> AppNotifications => Set<AppNotification>();
    #endregion

    public DbSet<Lookup> Lookups => Set<Lookup>();

    public DbSet<LookupDetail> LookupDetails => Set<LookupDetail>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}
