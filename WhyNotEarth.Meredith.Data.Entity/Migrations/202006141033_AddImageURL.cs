using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WhyNotEarth.Meredith.Data.Entity.Migrations
{
    public partial class AddImageURLToEndpoints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageURL",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "ImageURL",
                table: "ProductCategories");
            
            migrationBuilder.AddColumn<string>(
                name: "ImageURL",
                table: "Tenants");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageURL",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ImageURL",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "ImageURL",
                table: "Tenants");
        }
    }
}
