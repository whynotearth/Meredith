using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class RenameArticleImageCaptionToExcerpt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageCaption",
                schema: "ModuleVolkswagen",
                table: "Articles",
                newName: "Excerpt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Excerpt",
                schema: "ModuleVolkswagen",
                table: "Articles",
                newName: "ImageCaption");
        }
    }
}