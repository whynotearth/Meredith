using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Data.Entity.Migrations
{
    public partial class AddRecipientProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DistributionGroup",
                schema: "ModuleVolkswagen",
                table: "Recipients",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "ModuleVolkswagen",
                table: "Recipients",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                schema: "ModuleVolkswagen",
                table: "Recipients",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistributionGroup",
                schema: "ModuleVolkswagen",
                table: "Recipients");

            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "ModuleVolkswagen",
                table: "Recipients");

            migrationBuilder.DropColumn(
                name: "LastName",
                schema: "ModuleVolkswagen",
                table: "Recipients");
        }
    }
}
