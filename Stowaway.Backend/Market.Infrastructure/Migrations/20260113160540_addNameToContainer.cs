using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stowaway.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addNameToContainer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "Storage",
                table: "Container",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                schema: "Storage",
                table: "Container");
        }
    }
}
