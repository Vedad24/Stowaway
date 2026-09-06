using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stowaway.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddItemUserFavourite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Item_UserFavourite",
                schema: "Storage",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item_UserFavourite", x => new { x.ItemId, x.UserId });
                    table.ForeignKey(
                        name: "FK_Item_UserFavourite_Item_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "Storage",
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Item_UserFavourite_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Item_UserFavourite_UserId",
                schema: "Storage",
                table: "Item_UserFavourite",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Item_UserFavourite",
                schema: "Storage");
        }
    }
}
