using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class AddPmuPdfToClient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PmuPdf",
                schema: "ModuleBrowTricks",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SignatureRequestId",
                schema: "ModuleBrowTricks",
                table: "Clients",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PmuPdf",
                schema: "ModuleBrowTricks",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "SignatureRequestId",
                schema: "ModuleBrowTricks",
                table: "Clients");
        }
    }
}
