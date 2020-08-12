using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System;

namespace WhyNotEarth.Meredith.Data.Entity.Migrations
{
    public partial class AddPricing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Prices",
                schema: "ModuleHotel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Amount = table.Column<decimal>(type: "numeric(5, 2)", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    HotelId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prices_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalSchema: "ModuleHotel",
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Prices_HotelId",
                schema: "ModuleHotel",
                table: "Prices",
                column: "HotelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Prices",
                schema: "ModuleHotel");
        }
    }
}
