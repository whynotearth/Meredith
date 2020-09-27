using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class AddedFileSizeToVideoAndImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "FileSize",
                schema: "public",
                table: "Videos",
                nullable: true);

            migrationBuilder.AddColumn<double>(
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
