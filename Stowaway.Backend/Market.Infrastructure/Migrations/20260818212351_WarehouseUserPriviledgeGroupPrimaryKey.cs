using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stowaway.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class WarehouseUserPriviledgeGroupPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Warehouse_User",
                schema: "StorageIdentity",
                table: "Warehouse_User");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Warehouse_User",
                schema: "StorageIdentity",
                table: "Warehouse_User",
                columns: new[] { "WarehouseId", "UserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Warehouse_User",
                schema: "StorageIdentity",
                table: "Warehouse_User");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Warehouse_User",
                schema: "StorageIdentity",
                table: "Warehouse_User",
                columns: new[] { "WarehouseId", "UserId", "PriviledgeGroupId" });
        }
    }
}
