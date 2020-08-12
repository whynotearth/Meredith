using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class TweakNameAndPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "ModuleHotel",
                table: "RoomTypes",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                schema: "ModuleHotel",
                table: "Prices",
                type: "numeric(10, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(5, 2)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "ModuleHotel",
                table: "RoomTypes",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                schema: "ModuleHotel",
                table: "Prices",
                type: "numeric(5, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10, 2)");
        }
    }
}
