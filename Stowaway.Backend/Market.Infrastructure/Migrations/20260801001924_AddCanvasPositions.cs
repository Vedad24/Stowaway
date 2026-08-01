using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stowaway.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCanvasPositions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CanvasX",
                schema: "Storage",
                table: "Item",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CanvasY",
                schema: "Storage",
                table: "Item",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CanvasX",
                schema: "Storage",
                table: "Container",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CanvasY",
                schema: "Storage",
                table: "Container",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanvasX",
                schema: "Storage",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "CanvasY",
                schema: "Storage",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "CanvasX",
                schema: "Storage",
                table: "Container");

            migrationBuilder.DropColumn(
                name: "CanvasY",
                schema: "Storage",
                table: "Container");
        }
    }
}
