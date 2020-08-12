using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Data.Entity.Migrations
{
    public partial class RenameEmailRecipientToEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailRecipients_Companies_CompanyId",
                schema: "public",
                table: "EmailRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailRecipients_JumpStarts_JumpStartId",
                schema: "public",
                table: "EmailRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailRecipients_Memos_MemoId",
                schema: "public",
                table: "EmailRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailRecipients_NewJumpStarts_NewJumpStartId",
                schema: "public",
                table: "EmailRecipients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmailRecipients",
                schema: "public",
                table: "EmailRecipients");

            migrationBuilder.RenameTable(
                name: "EmailRecipients",
                schema: "public",
                newName: "Emails",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_EmailRecipients_NewJumpStartId",
                schema: "public",
                table: "Emails",
                newName: "IX_Emails_NewJumpStartId");

            migrationBuilder.RenameIndex(
                name: "IX_EmailRecipients_MemoId",
                schema: "public",
                table: "Emails",
                newName: "IX_Emails_MemoId");

            migrationBuilder.RenameIndex(
                name: "IX_EmailRecipients_JumpStartId",
                schema: "public",
                table: "Emails",
                newName: "IX_Emails_JumpStartId");

            migrationBuilder.RenameIndex(
                name: "IX_EmailRecipients_CompanyId",
                schema: "public",
                table: "Emails",
                newName: "IX_Emails_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Emails",
                schema: "public",
                table: "Emails",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Emails_Companies_CompanyId",
                schema: "public",
                table: "Emails",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Emails_JumpStarts_JumpStartId",
                schema: "public",
                table: "Emails",
                column: "JumpStartId",
                principalSchema: "ModuleVolkswagen",
                principalTable: "JumpStarts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Emails_Memos_MemoId",
                schema: "public",
                table: "Emails",
                column: "MemoId",
                principalSchema: "ModuleVolkswagen",
                principalTable: "Memos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Emails_NewJumpStarts_NewJumpStartId",
                schema: "public",
                table: "Emails",
                column: "NewJumpStartId",
                principalSchema: "ModuleVolkswagen",
                principalTable: "NewJumpStarts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emails_Companies_CompanyId",
                schema: "public",
                table: "Emails");

            migrationBuilder.DropForeignKey(
                name: "FK_Emails_JumpStarts_JumpStartId",
                schema: "public",
                table: "Emails");

            migrationBuilder.DropForeignKey(
                name: "FK_Emails_Memos_MemoId",
                schema: "public",
                table: "Emails");

            migrationBuilder.DropForeignKey(
                name: "FK_Emails_NewJumpStarts_NewJumpStartId",
                schema: "public",
                table: "Emails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Emails",
                schema: "public",
                table: "Emails");

            migrationBuilder.RenameTable(
                name: "Emails",
                schema: "public",
                newName: "EmailRecipients",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_Emails_NewJumpStartId",
                schema: "public",
                table: "EmailRecipients",
                newName: "IX_EmailRecipients_NewJumpStartId");

            migrationBuilder.RenameIndex(
                name: "IX_Emails_MemoId",
                schema: "public",
                table: "EmailRecipients",
                newName: "IX_EmailRecipients_MemoId");

            migrationBuilder.RenameIndex(
                name: "IX_Emails_JumpStartId",
                schema: "public",
                table: "EmailRecipients",
                newName: "IX_EmailRecipients_JumpStartId");

            migrationBuilder.RenameIndex(
                name: "IX_Emails_CompanyId",
                schema: "public",
                table: "EmailRecipients",
                newName: "IX_EmailRecipients_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmailRecipients",
                schema: "public",
                table: "EmailRecipients",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailRecipients_Companies_CompanyId",
                schema: "public",
                table: "EmailRecipients",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailRecipients_JumpStarts_JumpStartId",
                schema: "public",
                table: "EmailRecipients",
                column: "JumpStartId",
                principalSchema: "ModuleVolkswagen",
                principalTable: "JumpStarts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailRecipients_Memos_MemoId",
                schema: "public",
                table: "EmailRecipients",
                column: "MemoId",
                principalSchema: "ModuleVolkswagen",
                principalTable: "Memos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailRecipients_NewJumpStarts_NewJumpStartId",
                schema: "public",
                table: "EmailRecipients",
                column: "NewJumpStartId",
                principalSchema: "ModuleVolkswagen",
                principalTable: "NewJumpStarts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
