using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stowaway.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PriviledgeUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "StorageIdentity",
                table: "PriviledgeGroup",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "WarehouseId",
                schema: "StorageIdentity",
                table: "PriviledgeGroup",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "StorageIdentity",
                table: "Priviledge",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                schema: "StorageIdentity",
                table: "Priviledge",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PriviledgeGroupEntityId",
                schema: "StorageIdentity",
                table: "Priviledge",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "Identity",
                table: "Permission",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_PriviledgeGroup_WarehouseId",
                schema: "StorageIdentity",
                table: "PriviledgeGroup",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Priviledge_PriviledgeGroupEntityId",
                schema: "StorageIdentity",
                table: "Priviledge",
                column: "PriviledgeGroupEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Priviledge_PriviledgeGroup_PriviledgeGroupEntityId",
                schema: "StorageIdentity",
                table: "Priviledge",
                column: "PriviledgeGroupEntityId",
                principalSchema: "StorageIdentity",
                principalTable: "PriviledgeGroup",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PriviledgeGroup_Warehouse_WarehouseId",
                schema: "StorageIdentity",
                table: "PriviledgeGroup",
                column: "WarehouseId",
                principalSchema: "Storage",
                principalTable: "Warehouse",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Priviledge_PriviledgeGroup_PriviledgeGroupEntityId",
                schema: "StorageIdentity",
                table: "Priviledge");

            migrationBuilder.DropForeignKey(
                name: "FK_PriviledgeGroup_Warehouse_WarehouseId",
                schema: "StorageIdentity",
                table: "PriviledgeGroup");

            migrationBuilder.DropIndex(
                name: "IX_PriviledgeGroup_WarehouseId",
                schema: "StorageIdentity",
                table: "PriviledgeGroup");

            migrationBuilder.DropIndex(
                name: "IX_Priviledge_PriviledgeGroupEntityId",
                schema: "StorageIdentity",
                table: "Priviledge");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "StorageIdentity",
                table: "PriviledgeGroup");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                schema: "StorageIdentity",
                table: "PriviledgeGroup");

            migrationBuilder.DropColumn(
                name: "Code",
                schema: "StorageIdentity",
                table: "Priviledge");

            migrationBuilder.DropColumn(
                name: "PriviledgeGroupEntityId",
                schema: "StorageIdentity",
                table: "Priviledge");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "StorageIdentity",
                table: "Priviledge",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "Identity",
                table: "Permission",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");
        }
    }
}
