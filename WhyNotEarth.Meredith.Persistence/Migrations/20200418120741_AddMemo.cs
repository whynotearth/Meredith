using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class AddMemo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Memos",
                schema: "ModuleVolkswagen",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Subject = table.Column<string>(nullable: false),
                    Date = table.Column<string>(nullable: false),
                    To = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    DistributionGroup = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MemoRecipients",
                schema: "ModuleVolkswagen",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    MemoId = table.Column<int>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    Status = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemoRecipients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemoRecipients_Memos_MemoId",
                        column: x => x.MemoId,
                        principalSchema: "ModuleVolkswagen",
                        principalTable: "Memos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemoRecipients_MemoId",
                schema: "ModuleVolkswagen",
                table: "MemoRecipients",
                column: "MemoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemoRecipients",
                schema: "ModuleVolkswagen");

            migrationBuilder.DropTable(
                name: "Memos",
                schema: "ModuleVolkswagen");
        }
    }
}