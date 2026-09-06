using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stowaway.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixUserRefreshTokenCartItemSchemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_ContainerType_ContainerTypeId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ContainerStatusHistory_Users_UserId",
                schema: "Storage",
                table: "ContainerStatusHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Users_UserId",
                schema: "Sales",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Role_RoleId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Warehouse_User_Users_UserId",
                schema: "StorageIdentity",
                table: "Warehouse_User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CartItems",
                table: "CartItems");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User",
                newSchema: "Identity");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                newName: "RefreshToken",
                newSchema: "Identity");

            migrationBuilder.RenameTable(
                name: "CartItems",
                newName: "CartItem",
                newSchema: "Sales");

            migrationBuilder.RenameIndex(
                name: "IX_Users_RoleId",
                schema: "Identity",
                table: "User",
                newName: "IX_User_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Email",
                schema: "Identity",
                table: "User",
                newName: "IX_User_Email");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_UserId_TokenHash",
                schema: "Identity",
                table: "RefreshToken",
                newName: "IX_RefreshToken_UserId_TokenHash");

            migrationBuilder.RenameIndex(
                name: "IX_CartItems_ContainerTypeId",
                schema: "Sales",
                table: "CartItem",
                newName: "IX_CartItem_ContainerTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                schema: "Identity",
                table: "User",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshToken",
                schema: "Identity",
                table: "RefreshToken",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CartItem",
                schema: "Sales",
                table: "CartItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_ContainerType_ContainerTypeId",
                schema: "Sales",
                table: "CartItem",
                column: "ContainerTypeId",
                principalSchema: "Storage",
                principalTable: "ContainerType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContainerStatusHistory_User_UserId",
                schema: "Storage",
                table: "ContainerStatusHistory",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_User_UserId",
                schema: "Sales",
                table: "Order",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshToken_User_UserId",
                schema: "Identity",
                table: "RefreshToken",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Role_RoleId",
                schema: "Identity",
                table: "User",
                column: "RoleId",
                principalSchema: "Identity",
                principalTable: "Role",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouse_User_User_UserId",
                schema: "StorageIdentity",
                table: "Warehouse_User",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_ContainerType_ContainerTypeId",
                schema: "Sales",
                table: "CartItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ContainerStatusHistory_User_UserId",
                schema: "Storage",
                table: "ContainerStatusHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_User_UserId",
                schema: "Sales",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_User_UserId",
                schema: "Identity",
                table: "RefreshToken");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Role_RoleId",
                schema: "Identity",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_Warehouse_User_User_UserId",
                schema: "StorageIdentity",
                table: "Warehouse_User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                schema: "Identity",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshToken",
                schema: "Identity",
                table: "RefreshToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CartItem",
                schema: "Sales",
                table: "CartItem");

            migrationBuilder.RenameTable(
                name: "User",
                schema: "Identity",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "RefreshToken",
                schema: "Identity",
                newName: "RefreshTokens");

            migrationBuilder.RenameTable(
                name: "CartItem",
                schema: "Sales",
                newName: "CartItems");

            migrationBuilder.RenameIndex(
                name: "IX_User_RoleId",
                table: "Users",
                newName: "IX_Users_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_User_Email",
                table: "Users",
                newName: "IX_Users_Email");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshToken_UserId_TokenHash",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_UserId_TokenHash");

            migrationBuilder.RenameIndex(
                name: "IX_CartItem_ContainerTypeId",
                table: "CartItems",
                newName: "IX_CartItems_ContainerTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CartItems",
                table: "CartItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_ContainerType_ContainerTypeId",
                table: "CartItems",
                column: "ContainerTypeId",
                principalSchema: "Storage",
                principalTable: "ContainerType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContainerStatusHistory_Users_UserId",
                schema: "Storage",
                table: "ContainerStatusHistory",
                column: "UserId",
                principalTable: "Users",
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

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Role_RoleId",
                table: "Users",
                column: "RoleId",
                principalSchema: "Identity",
                principalTable: "Role",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouse_User_Users_UserId",
                schema: "StorageIdentity",
                table: "Warehouse_User",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
