using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Data.Entity.Migrations
{
    public partial class AddImageToProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                schema: "ModuleShop",
                table: "Products",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_ImageId",
                schema: "ModuleShop",
                table: "Products",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Images_ImageId",
                schema: "ModuleShop",
                table: "Products",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Images_ImageId",
                schema: "ModuleShop",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_ImageId",
                schema: "ModuleShop",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ImageId",
                schema: "ModuleShop",
                table: "Products");
        }
    }
}
