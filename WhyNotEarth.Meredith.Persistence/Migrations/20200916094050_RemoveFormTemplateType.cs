using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class RemoveFormTemplateType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                schema: "ModuleBrowTricks",
                table: "FormTemplates");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "ModuleBrowTricks",
                table: "FormSignatures");

            migrationBuilder.Sql("DELETE FROM \"ModuleBrowTricks\".\"FormSignatures\"");

            migrationBuilder.AddColumn<int>(
                name: "FormTemplateId",
                schema: "ModuleBrowTricks",
                table: "FormSignatures",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FormSignatures_FormTemplateId",
                schema: "ModuleBrowTricks",
                table: "FormSignatures",
                column: "FormTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormSignatures_FormTemplates_FormTemplateId",
                schema: "ModuleBrowTricks",
                table: "FormSignatures",
                column: "FormTemplateId",
                principalSchema: "ModuleBrowTricks",
                principalTable: "FormTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormSignatures_FormTemplates_FormTemplateId",
                schema: "ModuleBrowTricks",
                table: "FormSignatures");

            migrationBuilder.DropIndex(
                name: "IX_FormSignatures_FormTemplateId",
                schema: "ModuleBrowTricks",
                table: "FormSignatures");

            migrationBuilder.DropColumn(
                name: "FormTemplateId",
                schema: "ModuleBrowTricks",
                table: "FormSignatures");

            migrationBuilder.Sql("DELETE FROM \"ModuleBrowTricks\".\"FormSignatures\"");

            migrationBuilder.AddColumn<byte>(
                name: "Type",
                schema: "ModuleBrowTricks",
                table: "FormTemplates",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "Type",
                schema: "ModuleBrowTricks",
                table: "FormSignatures",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
