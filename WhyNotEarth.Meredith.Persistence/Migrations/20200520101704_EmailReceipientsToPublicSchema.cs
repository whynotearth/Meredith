using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class EmailReceipientsToPublicSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "EmailRecipients",
                schema: "ModuleVolkswagen",
                newName: "EmailRecipients",
                newSchema: "public");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "EmailRecipients",
                schema: "public",
                newName: "EmailRecipients",
                newSchema: "ModuleVolkswagen");
        }
    }
}