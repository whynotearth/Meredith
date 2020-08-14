using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class MoveRecipientsToVolkswagen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Recipients",
                newName: "Recipients",
                newSchema: "ModuleVolkswagen");

            // Custom
            migrationBuilder.UpdateData("Roles", "Id", 2,
                new[] { "Name", "NormalizedName" },
                new object[] { "VolkswagenManager", "VOLKSWAGENMANAGER" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Recipients",
                schema: "ModuleVolkswagen",
                newName: "Recipients");

            // Custom
            migrationBuilder.UpdateData("Roles", "Id", 2,
                new[] { "Name", "NormalizedName" },
                new object[] { "VolkswagenAdmin", "VOLKSWAGENADMIN" });
        }
    }
}