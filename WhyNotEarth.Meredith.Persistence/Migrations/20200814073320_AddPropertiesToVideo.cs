using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class AddPropertiesToVideo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Duration",
                schema: "public",
                table: "Videos",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Format",
                schema: "public",
                table: "Videos",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Height",
                schema: "public",
                table: "Videos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                schema: "public",
                table: "Videos",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Width",
                schema: "public",
                table: "Videos",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                schema: "public",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "Format",
                schema: "public",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "Height",
                schema: "public",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                schema: "public",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "Width",
                schema: "public",
                table: "Videos");
        }
    }
}
