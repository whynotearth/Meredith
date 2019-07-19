using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WhyNotEarth.Meredith.Data.Entity.Migrations
{
    public partial class AddReservationsRooms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Amenities_Hotels_HotelId",
                schema: "ModuleHotel",
                table: "Amenities");

            migrationBuilder.DropForeignKey(
                name: "FK_Beds_Hotels_HotelId",
                schema: "ModuleHotel",
                table: "Beds");

            migrationBuilder.DropForeignKey(
                name: "FK_Prices_Hotels_HotelId",
                schema: "ModuleHotel",
                table: "Prices");

            migrationBuilder.RenameColumn(
                name: "HotelId",
                schema: "ModuleHotel",
                table: "Prices",
                newName: "RoomTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Prices_HotelId",
                schema: "ModuleHotel",
                table: "Prices",
                newName: "IX_Prices_RoomTypeId");

            migrationBuilder.RenameColumn(
                name: "HotelId",
                schema: "ModuleHotel",
                table: "Beds",
                newName: "RoomTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Beds_HotelId",
                schema: "ModuleHotel",
                table: "Beds",
                newName: "IX_Beds_RoomTypeId");

            migrationBuilder.RenameColumn(
                name: "HotelId",
                schema: "ModuleHotel",
                table: "Amenities",
                newName: "RoomTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Amenities_HotelId",
                schema: "ModuleHotel",
                table: "Amenities",
                newName: "IX_Amenities_RoomTypeId");

            migrationBuilder.CreateTable(
                name: "RoomTypes",
                schema: "ModuleHotel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    HotelId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomTypes_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalSchema: "ModuleHotel",
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                schema: "ModuleHotel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    RoomTypeId = table.Column<int>(nullable: false),
                    Number = table.Column<string>(maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_RoomTypes_RoomTypeId",
                        column: x => x.RoomTypeId,
                        principalSchema: "ModuleHotel",
                        principalTable: "RoomTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                schema: "ModuleHotel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Amount = table.Column<decimal>(type: "numeric(5, 2)", nullable: false),
                    End = table.Column<DateTime>(type: "date", nullable: false),
                    RoomId = table.Column<int>(nullable: false),
                    Start = table.Column<DateTime>(type: "date", nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalSchema: "ModuleHotel",
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_RoomId",
                schema: "ModuleHotel",
                table: "Reservations",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_UserId",
                schema: "ModuleHotel",
                table: "Reservations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_RoomTypeId",
                schema: "ModuleHotel",
                table: "Rooms",
                column: "RoomTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomTypes_HotelId",
                schema: "ModuleHotel",
                table: "RoomTypes",
                column: "HotelId");

            migrationBuilder.Sql(@"
                INSERT INTO
                    ""ModuleHotel"".""RoomTypes""
                (""Name"", ""HotelId"")
                SELECT 'Default', ""Id"" FROM ""ModuleHotel"".""Hotels""
            ");

            migrationBuilder.Sql(@"
                UPDATE ""ModuleHotel"".""Prices"" P
                SET 
                    ""RoomTypeId"" = RT.""Id""
                FROM
                    ""ModuleHotel"".""Hotels"" H
                LEFT JOIN ""ModuleHotel"".""RoomTypes"" RT
                    ON RT.""HotelId"" = H.""Id""
                WHERE
                    H.""Id"" = P.""RoomTypeId""
            ");

            migrationBuilder.AddForeignKey(
                name: "FK_Amenities_RoomTypes_RoomTypeId",
                schema: "ModuleHotel",
                table: "Amenities",
                column: "RoomTypeId",
                principalSchema: "ModuleHotel",
                principalTable: "RoomTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Beds_RoomTypes_RoomTypeId",
                schema: "ModuleHotel",
                table: "Beds",
                column: "RoomTypeId",
                principalSchema: "ModuleHotel",
                principalTable: "RoomTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prices_RoomTypes_RoomTypeId",
                schema: "ModuleHotel",
                table: "Prices",
                column: "RoomTypeId",
                principalSchema: "ModuleHotel",
                principalTable: "RoomTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Amenities_RoomTypes_RoomTypeId",
                schema: "ModuleHotel",
                table: "Amenities");

            migrationBuilder.DropForeignKey(
                name: "FK_Beds_RoomTypes_RoomTypeId",
                schema: "ModuleHotel",
                table: "Beds");

            migrationBuilder.DropForeignKey(
                name: "FK_Prices_RoomTypes_RoomTypeId",
                schema: "ModuleHotel",
                table: "Prices");

            migrationBuilder.DropTable(
                name: "Reservations",
                schema: "ModuleHotel");

            migrationBuilder.DropTable(
                name: "Rooms",
                schema: "ModuleHotel");

            migrationBuilder.DropTable(
                name: "RoomTypes",
                schema: "ModuleHotel");

            migrationBuilder.RenameColumn(
                name: "RoomTypeId",
                schema: "ModuleHotel",
                table: "Prices",
                newName: "HotelId");

            migrationBuilder.RenameIndex(
                name: "IX_Prices_RoomTypeId",
                schema: "ModuleHotel",
                table: "Prices",
                newName: "IX_Prices_HotelId");

            migrationBuilder.RenameColumn(
                name: "RoomTypeId",
                schema: "ModuleHotel",
                table: "Beds",
                newName: "HotelId");

            migrationBuilder.RenameIndex(
                name: "IX_Beds_RoomTypeId",
                schema: "ModuleHotel",
                table: "Beds",
                newName: "IX_Beds_HotelId");

            migrationBuilder.RenameColumn(
                name: "RoomTypeId",
                schema: "ModuleHotel",
                table: "Amenities",
                newName: "HotelId");

            migrationBuilder.RenameIndex(
                name: "IX_Amenities_RoomTypeId",
                schema: "ModuleHotel",
                table: "Amenities",
                newName: "IX_Amenities_HotelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Amenities_Hotels_HotelId",
                schema: "ModuleHotel",
                table: "Amenities",
                column: "HotelId",
                principalSchema: "ModuleHotel",
                principalTable: "Hotels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Beds_Hotels_HotelId",
                schema: "ModuleHotel",
                table: "Beds",
                column: "HotelId",
                principalSchema: "ModuleHotel",
                principalTable: "Hotels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prices_Hotels_HotelId",
                schema: "ModuleHotel",
                table: "Prices",
                column: "HotelId",
                principalSchema: "ModuleHotel",
                principalTable: "Hotels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
