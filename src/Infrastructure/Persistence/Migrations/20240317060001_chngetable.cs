using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitechture.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class chngetable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProcessedDate",
                table: "OutboxMessage",
                newName: "ProcessedOn");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "OutboxMessage",
                newName: "CreatedOn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProcessedOn",
                table: "OutboxMessage",
                newName: "ProcessedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "OutboxMessage",
                newName: "CreatedDate");
        }
    }
}
