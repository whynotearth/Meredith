using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class AddReservationFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "ModuleHotel",
                table: "Reservations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                schema: "ModuleHotel",
                table: "Reservations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "ModuleHotel",
                table: "Reservations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfGuests",
                schema: "ModuleHotel",
                table: "Reservations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                schema: "ModuleHotel",
                table: "Reservations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                schema: "ModuleHotel",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Message",
                schema: "ModuleHotel",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "ModuleHotel",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "NumberOfGuests",
                schema: "ModuleHotel",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Phone",
                schema: "ModuleHotel",
                table: "Reservations");
        }
    }
}
