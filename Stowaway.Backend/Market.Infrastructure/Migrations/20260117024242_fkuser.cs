using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stowaway.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fkuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_OrderStatus_OrderStatusId1",
                schema: "Sales",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_OrderStatusId1",
                schema: "Sales",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderStatusId1",
                schema: "Sales",
                table: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_Order_OrderStatusId",
                schema: "Sales",
                table: "Order",
                column: "OrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_UserId",
                schema: "Sales",
                table: "Order",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_OrderStatus_OrderStatusId",
                schema: "Sales",
                table: "Order",
                column: "OrderStatusId",
                principalSchema: "Sales",
                principalTable: "OrderStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Users_UserId",
                schema: "Sales",
                table: "Order",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_OrderStatus_OrderStatusId",
                schema: "Sales",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Users_UserId",
                schema: "Sales",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_OrderStatusId",
                schema: "Sales",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_UserId",
                schema: "Sales",
                table: "Order");

            migrationBuilder.AddColumn<int>(
                name: "OrderStatusId1",
                schema: "Sales",
                table: "Order",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_OrderStatusId1",
                schema: "Sales",
                table: "Order",
                column: "OrderStatusId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_OrderStatus_OrderStatusId1",
                schema: "Sales",
                table: "Order",
                column: "OrderStatusId1",
                principalSchema: "Sales",
                principalTable: "OrderStatus",
                principalColumn: "Id");
        }
    }
}
