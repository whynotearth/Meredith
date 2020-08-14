using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class RenameMemoRecipientTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemoRecipients_Memos_MemoId",
                schema: "ModuleVolkswagen",
                table: "MemoRecipients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MemoRecipients",
                schema: "ModuleVolkswagen",
                table: "MemoRecipients");

            migrationBuilder.RenameTable(
                name: "MemoRecipients",
                schema: "ModuleVolkswagen",
                newName: "EmailRecipients",
                newSchema: "ModuleVolkswagen");

            migrationBuilder.RenameIndex(
                name: "IX_MemoRecipients_MemoId",
                schema: "ModuleVolkswagen",
                table: "EmailRecipients",
                newName: "IX_EmailRecipients_MemoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmailRecipients",
                schema: "ModuleVolkswagen",
                table: "EmailRecipients",
                column: "Id");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailRecipients_Memos_MemoId",
                schema: "ModuleVolkswagen",
                table: "EmailRecipients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmailRecipients",
                schema: "ModuleVolkswagen",
                table: "EmailRecipients");

            migrationBuilder.RenameTable(
                name: "EmailRecipients",
                schema: "ModuleVolkswagen",
                newName: "MemoRecipients",
                newSchema: "ModuleVolkswagen");

            migrationBuilder.RenameIndex(
                name: "IX_EmailRecipients_MemoId",
                schema: "ModuleVolkswagen",
                table: "MemoRecipients",
                newName: "IX_MemoRecipients_MemoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MemoRecipients",
                schema: "ModuleVolkswagen",
                table: "MemoRecipients",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MemoRecipients_Memos_MemoId",
                schema: "ModuleVolkswagen",
                table: "MemoRecipients",
                column: "MemoId",
                principalSchema: "ModuleVolkswagen",
                principalTable: "Memos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}