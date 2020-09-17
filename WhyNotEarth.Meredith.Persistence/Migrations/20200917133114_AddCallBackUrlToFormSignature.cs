using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class AddCallBackUrlToFormSignature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NotificationCallBackUrl",
                schema: "ModuleBrowTricks",
                table: "FormSignatures",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotificationCallBackUrl",
                schema: "ModuleBrowTricks",
                table: "FormSignatures");
        }
    }
}
