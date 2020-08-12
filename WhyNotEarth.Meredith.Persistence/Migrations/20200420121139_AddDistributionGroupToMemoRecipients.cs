using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class AddDistributionGroupToMemoRecipients : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DistributionGroup",
                schema: "ModuleVolkswagen",
                table: "MemoRecipients",
                nullable: false,
                defaultValue: ""); // Custom);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistributionGroup",
                schema: "ModuleVolkswagen",
                table: "MemoRecipients");
        }
    }
}
