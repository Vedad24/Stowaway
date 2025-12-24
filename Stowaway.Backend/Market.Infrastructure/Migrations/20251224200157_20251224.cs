using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stowaway.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _20251224 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permission_Role_Permission_PermissionId1",
                schema: "Identity",
                table: "Permission_Role");

            migrationBuilder.DropForeignKey(
                name: "FK_Permission_Role_Role_RoleId1",
                schema: "Identity",
                table: "Permission_Role");

            migrationBuilder.DropIndex(
                name: "IX_Permission_Role_PermissionId1",
                schema: "Identity",
                table: "Permission_Role");

            migrationBuilder.DropIndex(
                name: "IX_Permission_Role_RoleId1",
                schema: "Identity",
                table: "Permission_Role");

            migrationBuilder.DropColumn(
                name: "PermissionId1",
                schema: "Identity",
                table: "Permission_Role");

            migrationBuilder.DropColumn(
                name: "RoleId1",
                schema: "Identity",
                table: "Permission_Role");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_Role_RoleId",
                schema: "Identity",
                table: "Permission_Role",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Permission_Role_Permission_PermissionId",
                schema: "Identity",
                table: "Permission_Role",
                column: "PermissionId",
                principalSchema: "Identity",
                principalTable: "Permission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Permission_Role_Role_RoleId",
                schema: "Identity",
                table: "Permission_Role",
                column: "RoleId",
                principalSchema: "Identity",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permission_Role_Permission_PermissionId",
                schema: "Identity",
                table: "Permission_Role");

            migrationBuilder.DropForeignKey(
                name: "FK_Permission_Role_Role_RoleId",
                schema: "Identity",
                table: "Permission_Role");

            migrationBuilder.DropIndex(
                name: "IX_Permission_Role_RoleId",
                schema: "Identity",
                table: "Permission_Role");

            migrationBuilder.AddColumn<int>(
                name: "PermissionId1",
                schema: "Identity",
                table: "Permission_Role",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RoleId1",
                schema: "Identity",
                table: "Permission_Role",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Permission_Role_PermissionId1",
                schema: "Identity",
                table: "Permission_Role",
                column: "PermissionId1");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_Role_RoleId1",
                schema: "Identity",
                table: "Permission_Role",
                column: "RoleId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Permission_Role_Permission_PermissionId1",
                schema: "Identity",
                table: "Permission_Role",
                column: "PermissionId1",
                principalSchema: "Identity",
                principalTable: "Permission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Permission_Role_Role_RoleId1",
                schema: "Identity",
                table: "Permission_Role",
                column: "RoleId1",
                principalSchema: "Identity",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
