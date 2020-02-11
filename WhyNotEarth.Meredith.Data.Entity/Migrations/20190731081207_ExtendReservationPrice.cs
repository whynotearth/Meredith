using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Data.Entity.Migrations
{
    public partial class ExtendReservationPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                schema: "ModuleHotel",
                table: "Reservations",
                type: "numeric(10, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(5, 2)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                schema: "ModuleHotel",
                table: "Reservations",
                type: "numeric(5, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10, 2)");
        }
    }
}
