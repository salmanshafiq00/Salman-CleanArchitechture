﻿// <auto-generated />
using System;
using CleanArchitechture.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CleanArchitechture.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240619212914_AddMenutypecolumn")]
    partial class AddMenutypecolumn
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CleanArchitechture.Domain.Admin.AppMenu", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Icon")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Label")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("MenuTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("OrderNo")
                        .HasColumnType("int");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Tooltip")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<bool>("Visible")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("Label")
                        .IsUnique();

                    b.HasIndex("MenuTypeId");

                    b.ToTable("AppMenus");
                });

            modelBuilder.Entity("CleanArchitechture.Domain.Admin.RoleMenu", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AppMenuId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("AppMenuId");

                    b.HasIndex("RoleId", "AppMenuId")
                        .IsUnique();

                    b.ToTable("RoleMenus");
                });

            modelBuilder.Entity("CleanArchitechture.Domain.Common.Lookup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int?>("DevCode")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("ParentId");

                    b.ToTable("Lookups");
                });

            modelBuilder.Entity("CleanArchitechture.Domain.Common.LookupDetail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<int?>("DevCode")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("LookupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("LookupId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("ParentId");

                    b.ToTable("LookupDetails");
                });

            modelBuilder.Entity("CleanArchitechture.Infrastructure.Persistence.Outbox.OutboxMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Error")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ProcessedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("OutboxMessage", (string)null);
                });

            modelBuilder.Entity("CleanArchitechture.Domain.Admin.AppMenu", b =>
                {
                    b.HasOne("CleanArchitechture.Domain.Common.LookupDetail", null)
                        .WithMany()
                        .HasForeignKey("MenuTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("CleanArchitechture.Domain.Admin.RoleMenu", b =>
                {
                    b.HasOne("CleanArchitechture.Domain.Admin.AppMenu", null)
                        .WithMany()
                        .HasForeignKey("AppMenuId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("CleanArchitechture.Domain.Common.Lookup", b =>
                {
                    b.HasOne("CleanArchitechture.Domain.Common.Lookup", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentId");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("CleanArchitechture.Domain.Common.LookupDetail", b =>
                {
                    b.HasOne("CleanArchitechture.Domain.Common.Lookup", "Lookup")
                        .WithMany()
                        .HasForeignKey("LookupId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("CleanArchitechture.Domain.Common.LookupDetail", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentId");

                    b.Navigation("Lookup");

                    b.Navigation("Parent");
                });
#pragma warning restore 612, 618
        }
    }
}
