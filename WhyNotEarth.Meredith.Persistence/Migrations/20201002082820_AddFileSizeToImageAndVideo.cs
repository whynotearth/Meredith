using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class AddFileSizeToImageAndVideo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                schema: "public",
                table: "Videos",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "Images",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileSize",
                schema: "public",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "Images");
        }
    }
}
