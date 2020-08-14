using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class AddPmuQuestions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PmuQuestions",
                schema: "ModuleBrowTricks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    TenantId = table.Column<int>(nullable: false),
                    Question = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PmuQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PmuQuestions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PmuAnswers",
                schema: "ModuleBrowTricks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    QuestionId = table.Column<int>(nullable: false),
                    ClientId = table.Column<int>(nullable: false),
                    Answer = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PmuAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PmuAnswers_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "ModuleBrowTricks",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PmuAnswers_PmuQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalSchema: "ModuleBrowTricks",
                        principalTable: "PmuQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PmuAnswers_ClientId",
                schema: "ModuleBrowTricks",
                table: "PmuAnswers",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_PmuAnswers_QuestionId",
                schema: "ModuleBrowTricks",
                table: "PmuAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_PmuQuestions_TenantId",
                schema: "ModuleBrowTricks",
                table: "PmuQuestions",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PmuAnswers",
                schema: "ModuleBrowTricks");

            migrationBuilder.DropTable(
                name: "PmuQuestions",
                schema: "ModuleBrowTricks");
        }
    }
}