using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class ChangeFormItemIdType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormItems",
                schema: "ModuleBrowTricks");

            migrationBuilder.Sql("TRUNCATE TABLE \"ModuleBrowTricks\".\"FormTemplates\"");

            migrationBuilder.CreateTable(
                name: "FormItems",
                schema: "ModuleBrowTricks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    FormTemplateId = table.Column<int>(type: "integer", nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    Options = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_FormItems_FormTemplateId",
                schema: "ModuleBrowTricks",
                table: "FormItems",
                column: "FormTemplateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormItems",
                schema: "ModuleBrowTricks");

            migrationBuilder.CreateTable(
                name: "FormItems",
                schema: "ModuleBrowTricks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FormTemplateId = table.Column<int>(type: "integer", nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    Options = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_FormItems_FormTemplateId",
                schema: "ModuleBrowTricks",
                table: "FormItems",
                column: "FormTemplateId");
        }
    }
}
