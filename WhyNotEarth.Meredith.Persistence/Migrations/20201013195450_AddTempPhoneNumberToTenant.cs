using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class AddTempPhoneNumberToTenant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TempPhoneNumber",
                table: "Tenants",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TempPhoneNumber",
                table: "Tenants");
        }
    }
}
