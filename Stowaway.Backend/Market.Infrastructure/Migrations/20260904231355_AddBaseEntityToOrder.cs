using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stowaway.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseEntityToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                schema: "Sales",
                table: "Order",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "Sales",
                table: "Order",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                schema: "Sales",
                table: "Order",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                schema: "Sales",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "Sales",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                schema: "Sales",
                table: "Order");
        }
    }
}
