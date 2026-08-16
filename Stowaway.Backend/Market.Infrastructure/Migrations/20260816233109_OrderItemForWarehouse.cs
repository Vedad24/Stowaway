using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stowaway.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OrderItemForWarehouse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WarehouseId",
                schema: "Sales",
                table: "OrderItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WarehouseId",
                table: "CartItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_WarehouseId",
                schema: "Sales",
                table: "OrderItem",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Warehouse_WarehouseId",
                schema: "Sales",
                table: "OrderItem",
                column: "WarehouseId",
                principalSchema: "Storage",
                principalTable: "Warehouse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Warehouse_WarehouseId",
                schema: "Sales",
                table: "OrderItem");

            migrationBuilder.DropIndex(
                name: "IX_OrderItem_WarehouseId",
                schema: "Sales",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                schema: "Sales",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "CartItems");
        }
    }
}
