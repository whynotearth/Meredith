using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Data.Entity.Migrations
{
    public partial class AddModuleHotel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ModuleHotel");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "Hotels",
                schema: "ModuleHotel",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Capacity = table.Column<int>(nullable: false),
                    GettingAround = table.Column<string>(nullable: true),
                    PageId = table.Column<Guid>(nullable: false),
                    Location = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hotels_Pages_PageId",
                        column: x => x.PageId,
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Amenities",
                schema: "ModuleHotel",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    HotelId = table.Column<Guid>(nullable: false),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amenities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Amenities_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalSchema: "ModuleHotel",
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Beds",
                schema: "ModuleHotel",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BedType = table.Column<int>(nullable: false),
                    Count = table.Column<int>(nullable: false),
                    HotelId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Beds_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalSchema: "ModuleHotel",
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rules",
                schema: "ModuleHotel",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    HotelId = table.Column<Guid>(nullable: false),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rules_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalSchema: "ModuleHotel",
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Spaces",
                schema: "ModuleHotel",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    HotelId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spaces_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalSchema: "ModuleHotel",
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Amenities_HotelId",
                schema: "ModuleHotel",
                table: "Amenities",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Beds_HotelId",
                schema: "ModuleHotel",
                table: "Beds",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_PageId",
                schema: "ModuleHotel",
                table: "Hotels",
                column: "PageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rules_HotelId",
                schema: "ModuleHotel",
                table: "Rules",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Spaces_HotelId",
                schema: "ModuleHotel",
                table: "Spaces",
                column: "HotelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Amenities",
                schema: "ModuleHotel");

            migrationBuilder.DropTable(
                name: "Beds",
                schema: "ModuleHotel");

            migrationBuilder.DropTable(
                name: "Rules",
                schema: "ModuleHotel");

            migrationBuilder.DropTable(
                name: "Spaces",
                schema: "ModuleHotel");

            migrationBuilder.DropTable(
                name: "Hotels",
                schema: "ModuleHotel");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");
        }
    }
}
