using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WhyNotEarth.Meredith.Data.Entity.Migrations
{
    public partial class OpeningHours : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OpeningHoursId",
                table: "Tenants",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OpeningHours",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    DayOneOpeningTime = table.Column<DateTime>(nullable: false),
                    DayOneClosingTime = table.Column<DateTime>(nullable: false),
                    DayTwoOpeningTime = table.Column<DateTime>(nullable: false),
                    DayTwoClosingTime = table.Column<DateTime>(nullable: false),
                    DayThreeOpeningTime = table.Column<DateTime>(nullable: false),
                    DayThreeClosingTime = table.Column<DateTime>(nullable: false),
                    DayFourOpeningTime = table.Column<DateTime>(nullable: false),
                    DayFourClosingTime = table.Column<DateTime>(nullable: false),
                    DayFiveOpeningTime = table.Column<DateTime>(nullable: false),
                    DayFiveClosingTime = table.Column<DateTime>(nullable: false),
                    DaySixOpeningTime = table.Column<DateTime>(nullable: false),
                    DaySixClosingTime = table.Column<DateTime>(nullable: false),
                    DaySevenOpeningTime = table.Column<DateTime>(nullable: false),
                    DaySevenClosingTime = table.Column<DateTime>(nullable: false),
                    OpenAlways = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpeningHours", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_OpeningHoursId",
                table: "Tenants",
                column: "OpeningHoursId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_OpeningHours_OpeningHoursId",
                table: "Tenants",
                column: "OpeningHoursId",
                principalTable: "OpeningHours",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_OpeningHours_OpeningHoursId",
                table: "Tenants");

            migrationBuilder.DropTable(
                name: "OpeningHours");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_OpeningHoursId",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "OpeningHoursId",
                table: "Tenants");
        }
    }
}
