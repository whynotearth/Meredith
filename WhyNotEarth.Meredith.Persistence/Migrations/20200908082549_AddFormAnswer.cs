using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class AddFormAnswer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Disclosures",
                schema: "ModuleBrowTricks");

            migrationBuilder.DropColumn(
                name: "Value",
                schema: "ModuleBrowTricks",
                table: "FormTemplates");

            migrationBuilder.DropColumn(
                name: "PmuPdf",
                schema: "ModuleBrowTricks",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "PmuStatus",
                schema: "ModuleBrowTricks",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "SignedAt",
                schema: "ModuleBrowTricks",
                table: "Clients");

            migrationBuilder.CreateTable(
                name: "FormItems",
                schema: "ModuleBrowTricks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FormTemplateId = table.Column<int>(nullable: false),
                    Type = table.Column<byte>(nullable: false),
                    IsRequired = table.Column<bool>(nullable: false),
                    Value = table.Column<string>(nullable: false),
                    Options = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormItems_FormTemplates_FormTemplateId",
                        column: x => x.FormTemplateId,
                        principalSchema: "ModuleBrowTricks",
                        principalTable: "FormTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormSignatures",
                schema: "ModuleBrowTricks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ClientId = table.Column<int>(nullable: false),
                    Type = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Html = table.Column<string>(nullable: true),
                    PdfPath = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormSignatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormSignatures_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "ModuleBrowTricks",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormAnswers",
                schema: "ModuleBrowTricks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    FormSignatureId = table.Column<int>(nullable: false),
                    Type = table.Column<byte>(nullable: false),
                    IsRequired = table.Column<bool>(nullable: false),
                    Question = table.Column<string>(nullable: false),
                    Options = table.Column<string>(nullable: true),
                    Answers = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormAnswers_FormSignatures_FormSignatureId",
                        column: x => x.FormSignatureId,
                        principalSchema: "ModuleBrowTricks",
                        principalTable: "FormSignatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormAnswers_FormSignatureId",
                schema: "ModuleBrowTricks",
                table: "FormAnswers",
                column: "FormSignatureId");

            migrationBuilder.CreateIndex(
                name: "IX_FormItems_FormTemplateId",
                schema: "ModuleBrowTricks",
                table: "FormItems",
                column: "FormTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_FormSignatures_ClientId",
                schema: "ModuleBrowTricks",
                table: "FormSignatures",
                column: "ClientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormAnswers",
                schema: "ModuleBrowTricks");

            migrationBuilder.DropTable(
                name: "FormItems",
                schema: "ModuleBrowTricks");

            migrationBuilder.DropTable(
                name: "FormSignatures",
                schema: "ModuleBrowTricks");

            migrationBuilder.AddColumn<string>(
                name: "Value",
                schema: "ModuleBrowTricks",
                table: "FormTemplates",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PmuPdf",
                schema: "ModuleBrowTricks",
                table: "Clients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PmuStatus",
                schema: "ModuleBrowTricks",
                table: "Clients",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SignedAt",
                schema: "ModuleBrowTricks",
                table: "Clients",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Disclosures",
                schema: "ModuleBrowTricks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disclosures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Disclosures_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Disclosures_TenantId",
                schema: "ModuleBrowTricks",
                table: "Disclosures",
                column: "TenantId");
        }
    }
}
