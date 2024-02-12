using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitechture.Infrastructure.Infrastructure.Data
{
    /// <inheritdoc />
    public partial class AddDetailLookup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Lookups",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<int>(
                name: "DevCode",
                table: "Lookups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Lookups",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "LookupDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LookupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DevCode = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LookupDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LookupDetails_LookupDetails_ParentId",
                        column: x => x.ParentId,
                        principalTable: "LookupDetails",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LookupDetails_Lookups_LookupId",
                        column: x => x.LookupId,
                        principalTable: "Lookups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lookups_Name",
                table: "Lookups",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LookupDetails_LookupId",
                table: "LookupDetails",
                column: "LookupId");

            migrationBuilder.CreateIndex(
                name: "IX_LookupDetails_Name",
                table: "LookupDetails",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LookupDetails_ParentId",
                table: "LookupDetails",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LookupDetails");

            migrationBuilder.DropIndex(
                name: "IX_Lookups_Name",
                table: "Lookups");

            migrationBuilder.DropColumn(
                name: "DevCode",
                table: "Lookups");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Lookups");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Lookups",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);
        }
    }
}
