using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System;

namespace WhyNotEarth.Meredith.Data.Entity.Migrations
{
    public partial class AddReservationCreatedDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                schema: "ModuleHotel",
                table: "Reservations",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                schema: "ModuleHotel",
                table: "Hotels",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Payments",
                schema: "ModuleHotel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Amount = table.Column<decimal>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    ReservationId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalSchema: "ModuleHotel",
                        principalTable: "Reservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_CompanyId",
                schema: "ModuleHotel",
                table: "Hotels",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ReservationId",
                schema: "ModuleHotel",
                table: "Payments",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserId",
                schema: "ModuleHotel",
                table: "Payments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hotels_Companies_CompanyId",
                schema: "ModuleHotel",
                table: "Hotels",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hotels_Companies_CompanyId",
                schema: "ModuleHotel",
                table: "Hotels");

            migrationBuilder.DropTable(
                name: "Payments",
                schema: "ModuleHotel");

            migrationBuilder.DropIndex(
                name: "IX_Hotels_CompanyId",
                schema: "ModuleHotel",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "Created",
                schema: "ModuleHotel",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "ModuleHotel",
                table: "Hotels");
        }
    }
}
