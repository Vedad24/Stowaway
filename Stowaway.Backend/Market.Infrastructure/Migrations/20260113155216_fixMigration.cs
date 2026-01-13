using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stowaway.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Container_Item",
                schema: "Storage");

            migrationBuilder.AddColumn<int>(
                name: "ContainerId",
                schema: "Storage",
                table: "Item",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Item_ContainerId",
                schema: "Storage",
                table: "Item",
                column: "ContainerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Item_Container_ContainerId",
                schema: "Storage",
                table: "Item",
                column: "ContainerId",
                principalSchema: "Storage",
                principalTable: "Container",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Item_Container_ContainerId",
                schema: "Storage",
                table: "Item");

            migrationBuilder.DropIndex(
                name: "IX_Item_ContainerId",
                schema: "Storage",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "ContainerId",
                schema: "Storage",
                table: "Item");

            migrationBuilder.CreateTable(
                name: "Container_Item",
                schema: "Storage",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ContainerId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Container_Item", x => new { x.ItemId, x.ContainerId });
                    table.ForeignKey(
                        name: "FK_Container_Item_Container_ContainerId",
                        column: x => x.ContainerId,
                        principalSchema: "Storage",
                        principalTable: "Container",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Container_Item_Item_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "Storage",
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Container_Item_ContainerId",
                schema: "Storage",
                table: "Container_Item",
                column: "ContainerId");
        }
    }
}
