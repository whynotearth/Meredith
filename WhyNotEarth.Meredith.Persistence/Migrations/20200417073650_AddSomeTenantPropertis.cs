using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class AddSomeTenantPropertis : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LogoId",
                table: "Tenants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Tenants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Tenants",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_LogoId",
                table: "Tenants",
                column: "LogoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_Images_LogoId",
                table: "Tenants",
                column: "LogoId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_Images_LogoId",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_LogoId",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "LogoId",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Tenants");
        }
    }
}