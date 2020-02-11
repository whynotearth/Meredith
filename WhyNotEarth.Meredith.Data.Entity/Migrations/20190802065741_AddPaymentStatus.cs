using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Data.Entity.Migrations
{
    public partial class AddPaymentStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentIntentId",
                schema: "ModuleHotel",
                table: "Payments",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "ModuleHotel",
                table: "Payments",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentIntentId",
                schema: "ModuleHotel",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "ModuleHotel",
                table: "Payments");
        }
    }
}
