using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Data.Entity.Migrations
{
    public partial class RenameEmailColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DistributionGroup",
                schema: "public",
                table: "Emails",
                newName: "Group");

            migrationBuilder.RenameColumn(
                name: "Email",
                schema: "public",
                table: "Emails",
                newName: "EmailAddress");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Group",
                schema: "public",
                table: "Emails",
                newName: "DistributionGroup");

            migrationBuilder.RenameColumn(
                name: "EmailAddress",
                schema: "public",
                table: "Emails",
                newName: "Email");
        }
    }
}
