using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class AddSignatureImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SignatureImage",
                schema: "ModuleBrowTricks",
                table: "FormSignatures",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SignatureImage",
                schema: "ModuleBrowTricks",
                table: "FormSignatures");
        }
    }
}
