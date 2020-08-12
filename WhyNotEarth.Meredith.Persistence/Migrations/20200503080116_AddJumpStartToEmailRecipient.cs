using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class AddJumpStartToEmailRecipient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailRecipients_Memos_MemoId",
                schema: "ModuleVolkswagen",
                table: "EmailRecipients");

            migrationBuilder.AlterColumn<int>(
                name: "MemoId",
                schema: "ModuleVolkswagen",
                table: "EmailRecipients",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "JumpStartId",
                schema: "ModuleVolkswagen",
                table: "EmailRecipients",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailRecipients_JumpStartId",
                schema: "ModuleVolkswagen",
                table: "EmailRecipients",
                column: "JumpStartId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailRecipients_JumpStarts_JumpStartId",
                schema: "ModuleVolkswagen",
                table: "EmailRecipients",
                column: "JumpStartId",
                principalSchema: "ModuleVolkswagen",
                principalTable: "JumpStarts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailRecipients_Memos_MemoId",
                schema: "ModuleVolkswagen",
                table: "EmailRecipients",
                column: "MemoId",
                principalSchema: "ModuleVolkswagen",
                principalTable: "Memos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailRecipients_JumpStarts_JumpStartId",
                schema: "ModuleVolkswagen",
                table: "EmailRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailRecipients_Memos_MemoId",
                schema: "ModuleVolkswagen",
                table: "EmailRecipients");

            migrationBuilder.DropIndex(
                name: "IX_EmailRecipients_JumpStartId",
                schema: "ModuleVolkswagen",
                table: "EmailRecipients");

            migrationBuilder.DropColumn(
                name: "JumpStartId",
                schema: "ModuleVolkswagen",
                table: "EmailRecipients");

            migrationBuilder.AlterColumn<int>(
                name: "MemoId",
                schema: "ModuleVolkswagen",
                table: "EmailRecipients",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailRecipients_Memos_MemoId",
                schema: "ModuleVolkswagen",
                table: "EmailRecipients",
                column: "MemoId",
                principalSchema: "ModuleVolkswagen",
                principalTable: "Memos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
