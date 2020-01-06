using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WhyNotEarth.Meredith.Data.Entity.Migrations
{
    public partial class AddMultiLanguageSupport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                schema: "ModuleHotel",
                table: "Spaces");

            migrationBuilder.DropColumn(
                name: "Text",
                schema: "ModuleHotel",
                table: "Rules");

            migrationBuilder.DropColumn(
                name: "GettingAround",
                schema: "ModuleHotel",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "Location",
                schema: "ModuleHotel",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "Text",
                schema: "ModuleHotel",
                table: "Amenities");

            migrationBuilder.CreateTable(
                name: "Languages",
                schema: "ModuleHotel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Culture = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AmenityTranslations",
                schema: "ModuleHotel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    AmenityId = table.Column<int>(nullable: false),
                    LanguageId = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmenityTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AmenityTranslations_Amenities_AmenityId",
                        column: x => x.AmenityId,
                        principalSchema: "ModuleHotel",
                        principalTable: "Amenities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AmenityTranslations_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "ModuleHotel",
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HotelTranslations",
                schema: "ModuleHotel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    HotelId = table.Column<int>(nullable: false),
                    LanguageId = table.Column<int>(nullable: false),
                    GettingAround = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HotelTranslations_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalSchema: "ModuleHotel",
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HotelTranslations_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "ModuleHotel",
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RuleTranslations",
                schema: "ModuleHotel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    RuleId = table.Column<int>(nullable: false),
                    LanguageId = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RuleTranslations_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "ModuleHotel",
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RuleTranslations_Rules_RuleId",
                        column: x => x.RuleId,
                        principalSchema: "ModuleHotel",
                        principalTable: "Rules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpaceTranslations",
                schema: "ModuleHotel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    SpaceId = table.Column<int>(nullable: false),
                    LanguageId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpaceTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpaceTranslations_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "ModuleHotel",
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpaceTranslations_Spaces_SpaceId",
                        column: x => x.SpaceId,
                        principalSchema: "ModuleHotel",
                        principalTable: "Spaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AmenityTranslations_AmenityId",
                schema: "ModuleHotel",
                table: "AmenityTranslations",
                column: "AmenityId");

            migrationBuilder.CreateIndex(
                name: "IX_AmenityTranslations_LanguageId",
                schema: "ModuleHotel",
                table: "AmenityTranslations",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_HotelTranslations_HotelId",
                schema: "ModuleHotel",
                table: "HotelTranslations",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_HotelTranslations_LanguageId",
                schema: "ModuleHotel",
                table: "HotelTranslations",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_RuleTranslations_LanguageId",
                schema: "ModuleHotel",
                table: "RuleTranslations",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_RuleTranslations_RuleId",
                schema: "ModuleHotel",
                table: "RuleTranslations",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_SpaceTranslations_LanguageId",
                schema: "ModuleHotel",
                table: "SpaceTranslations",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_SpaceTranslations_SpaceId",
                schema: "ModuleHotel",
                table: "SpaceTranslations",
                column: "SpaceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AmenityTranslations",
                schema: "ModuleHotel");

            migrationBuilder.DropTable(
                name: "HotelTranslations",
                schema: "ModuleHotel");

            migrationBuilder.DropTable(
                name: "RuleTranslations",
                schema: "ModuleHotel");

            migrationBuilder.DropTable(
                name: "SpaceTranslations",
                schema: "ModuleHotel");

            migrationBuilder.DropTable(
                name: "Languages",
                schema: "ModuleHotel");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "ModuleHotel",
                table: "Spaces",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Text",
                schema: "ModuleHotel",
                table: "Rules",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GettingAround",
                schema: "ModuleHotel",
                table: "Hotels",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                schema: "ModuleHotel",
                table: "Hotels",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Text",
                schema: "ModuleHotel",
                table: "Amenities",
                nullable: true);
        }
    }
}
