using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class AddNewJumpStart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ClickDateTime",
                schema: "public",
                table: "EmailRecipients",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NewJumpStartId",
                schema: "public",
                table: "EmailRecipients",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "SendGridAccounts",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NewJumpStarts",
                schema: "ModuleVolkswagen",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    DateTime = table.Column<DateTime>(nullable: false),
                    Subject = table.Column<string>(nullable: false),
                    DistributionGroups = table.Column<string>(nullable: false),
                    Tags = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: false),
                    Status = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewJumpStarts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailRecipients_NewJumpStartId",
                schema: "public",
                table: "EmailRecipients",
                column: "NewJumpStartId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailRecipients_NewJumpStarts_NewJumpStartId",
                schema: "public",
                table: "EmailRecipients");

            migrationBuilder.DropTable(
                name: "NewJumpStarts",
                schema: "ModuleVolkswagen");

            migrationBuilder.DropIndex(
                name: "IX_EmailRecipients_NewJumpStartId",
                schema: "public",
                table: "EmailRecipients");

            migrationBuilder.DropColumn(
                name: "ClickDateTime",
                schema: "public",
                table: "EmailRecipients");

            migrationBuilder.DropColumn(
                name: "NewJumpStartId",
                schema: "public",
                table: "EmailRecipients");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "SendGridAccounts");
        }
    }
}
