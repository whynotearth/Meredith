using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WhyNotEarth.Meredith.Data.Entity.Migrations
{
    public partial class AddShortMessageTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShortMessages",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CompanyId = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    From = table.Column<string>(nullable: false),
                    To = table.Column<string>(nullable: false),
                    Body = table.Column<string>(nullable: false),
                    IsWhatsApp = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    SentAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShortMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShortMessages_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShortMessages_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShortMessages_CompanyId",
                schema: "public",
                table: "ShortMessages",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ShortMessages_TenantId",
                schema: "public",
                table: "ShortMessages",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShortMessages",
                schema: "public");
        }
    }
}
