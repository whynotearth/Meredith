using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class MoveBusinessHoursToShopSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                schema: "ModuleShop",
                table: "Orders");

            migrationBuilder.RenameTable(
                name: "BusinessHours",
                schema: "public",
                newName: "BusinessHours",
                newSchema: "ModuleShop");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "BusinessHours",
                schema: "ModuleShop",
                newName: "BusinessHours",
                newSchema: "public");

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethodId",
                schema: "ModuleShop",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
