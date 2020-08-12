using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class LimitThePostImageToOne : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Posts_PostId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_PostId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "Images");

            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                schema: "ModuleVolkswagen",
                table: "Posts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ImageId",
                schema: "ModuleVolkswagen",
                table: "Posts",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_SendGridAccounts_CompanyId",
                table: "SendGridAccounts",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_SendGridAccounts_Companies_CompanyId",
                table: "SendGridAccounts",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Images_ImageId",
                schema: "ModuleVolkswagen",
                table: "Posts",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SendGridAccounts_Companies_CompanyId",
                table: "SendGridAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Images_ImageId",
                schema: "ModuleVolkswagen",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_ImageId",
                schema: "ModuleVolkswagen",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_SendGridAccounts_CompanyId",
                table: "SendGridAccounts");

            migrationBuilder.DropColumn(
                name: "ImageId",
                schema: "ModuleVolkswagen",
                table: "Posts");

            migrationBuilder.AddColumn<int>(
                name: "PostId",
                table: "Images",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_PostId",
                table: "Images",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Posts_PostId",
                table: "Images",
                column: "PostId",
                principalSchema: "ModuleVolkswagen",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
