using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stowaway.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StripeUpdate_OrderPaymentIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentIntentId",
                schema: "Sales",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeSessionId",
                schema: "Sales",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentIntentId",
                schema: "Sales",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "StripeSessionId",
                schema: "Sales",
                table: "Order");
        }
    }
}
