using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class SeparateClientMedia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotificationType",
                schema: "ModuleBrowTricks",
                table: "Clients");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "public",
                table: "Videos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Images",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                schema: "public",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Images");

            migrationBuilder.AddColumn<byte>(
                name: "NotificationType",
                schema: "ModuleBrowTricks",
                table: "Clients",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
