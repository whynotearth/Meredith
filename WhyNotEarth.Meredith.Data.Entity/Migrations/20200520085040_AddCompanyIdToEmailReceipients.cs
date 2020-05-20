using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Data.Entity.Migrations
{
    public partial class AddCompanyIdToEmailReceipients : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                schema: "ModuleVolkswagen",
                table: "EmailRecipients",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                "UPDATE \"ModuleVolkswagen\".\"EmailRecipients\"" +
                "SET \"CompanyId\" = (SELECT \"Id\" FROM public.\"Companies\" WHERE \"Slug\" = 'Volkswagen' limit 1)"
            );

            migrationBuilder.CreateIndex(
                name: "IX_EmailRecipients_CompanyId",
                schema: "ModuleVolkswagen",
                table: "EmailRecipients",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailRecipients_Companies_CompanyId",
                schema: "ModuleVolkswagen",
                table: "EmailRecipients",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailRecipients_Companies_CompanyId",
                schema: "ModuleVolkswagen",
                table: "EmailRecipients");

            migrationBuilder.DropIndex(
                name: "IX_EmailRecipients_CompanyId",
                schema: "ModuleVolkswagen",
                table: "EmailRecipients");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "ModuleVolkswagen",
                table: "EmailRecipients");
        }
    }
}
