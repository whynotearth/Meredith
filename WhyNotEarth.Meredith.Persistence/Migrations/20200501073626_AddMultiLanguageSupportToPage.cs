using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class AddMultiLanguageSupportToPage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.RenameTable(
                name: "Languages",
                schema: "ModuleHotel",
                newName: "Languages",
                newSchema: "public");

            migrationBuilder.CreateTable(
                name: "PageTranslation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    PageId = table.Column<int>(nullable: false),
                    LanguageId = table.Column<int>(nullable: false),
                    CallToAction = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Header = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageTranslation_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "public",
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PageTranslation_Pages_PageId",
                        column: x => x.PageId,
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PageTranslation_LanguageId",
                table: "PageTranslation",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_PageTranslation_PageId",
                table: "PageTranslation",
                column: "PageId");

            migrationBuilder.Sql(@"Insert INTO ""PageTranslation"" (""PageId"", ""LanguageId"", ""CallToAction"", ""Description"", ""Header"", ""Name"", ""Title"") 
                            SELECT ""Id"", '1', ""CallToAction"", ""Description"", ""Header"", ""Name"", ""Title"" FROM ""Pages""");

            migrationBuilder.DropColumn(
                name: "CallToAction",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "Header",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Pages");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageTranslation");

            migrationBuilder.RenameTable(
                name: "Languages",
                schema: "public",
                newName: "Languages",
                newSchema: "ModuleHotel");

            migrationBuilder.AddColumn<string>(
                name: "CallToAction",
                table: "Pages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Pages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Header",
                table: "Pages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Pages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Pages",
                type: "text",
                nullable: true);
        }
    }
}
