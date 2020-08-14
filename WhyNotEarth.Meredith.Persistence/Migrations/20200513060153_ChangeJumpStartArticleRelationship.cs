using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class ChangeJumpStartArticleRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_JumpStarts_JumpStartId",
                schema: "ModuleVolkswagen",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_JumpStartId",
                schema: "ModuleVolkswagen",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "JumpStartId",
                schema: "ModuleVolkswagen",
                table: "Articles");

            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                schema: "ModuleVolkswagen",
                table: "JumpStarts",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                schema: "ModuleVolkswagen",
                table: "JumpStarts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(byte));

            migrationBuilder.AddColumn<int>(
                name: "JumpStartId",
                schema: "ModuleVolkswagen",
                table: "Articles",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_JumpStartId",
                schema: "ModuleVolkswagen",
                table: "Articles",
                column: "JumpStartId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_JumpStarts_JumpStartId",
                schema: "ModuleVolkswagen",
                table: "Articles",
                column: "JumpStartId",
                principalSchema: "ModuleVolkswagen",
                principalTable: "JumpStarts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}