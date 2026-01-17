using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stowaway.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addAttributesToWarehouse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Adress",
                schema: "Storage",
                table: "Warehouse",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                schema: "Storage",
                table: "Warehouse",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "City",
                schema: "Storage",
                table: "Warehouse",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "Storage",
                table: "Warehouse",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "isEnabled",
                schema: "Storage",
                table: "Warehouse",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Adress",
                schema: "Storage",
                table: "Warehouse");

            migrationBuilder.DropColumn(
                name: "Capacity",
                schema: "Storage",
                table: "Warehouse");

            migrationBuilder.DropColumn(
                name: "City",
                schema: "Storage",
                table: "Warehouse");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "Storage",
                table: "Warehouse");

            migrationBuilder.DropColumn(
                name: "isEnabled",
                schema: "Storage",
                table: "Warehouse");
        }
    }
}
