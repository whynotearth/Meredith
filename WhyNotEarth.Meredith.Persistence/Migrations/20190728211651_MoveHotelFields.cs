using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class MoveHotelFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Amenities_RoomTypes_RoomTypeId",
                schema: "ModuleHotel",
                table: "Amenities");

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                schema: "ModuleHotel",
                table: "RoomTypes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
                UPDATE ""ModuleHotel"".""RoomTypes"" RT
                SET 
                    ""Capacity"" = H.""Capacity""
                FROM
                    ""ModuleHotel"".""Hotels"" H
                WHERE
                    H.""Id"" = RT.""HotelId""
            ");

            migrationBuilder.DropColumn(
                name: "Capacity",
                schema: "ModuleHotel",
                table: "Hotels");

            migrationBuilder.RenameColumn(
                name: "RoomTypeId",
                schema: "ModuleHotel",
                table: "Amenities",
                newName: "HotelId");

            migrationBuilder.Sql(@"
                UPDATE ""ModuleHotel"".""Amenities"" A
                SET 
                    ""HotelId"" = RT.""HotelId""
                FROM
                    ""ModuleHotel"".""RoomTypes"" RT
                WHERE
                    RT.""Id"" = A.""HotelId""
            ");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Amenities_Hotels_HotelId",
                schema: "ModuleHotel",
                table: "Amenities");

            migrationBuilder.DropColumn(
                name: "Capacity",
                schema: "ModuleHotel",
                table: "RoomTypes");

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

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                schema: "ModuleHotel",
                table: "Hotels",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Amenities_RoomTypes_RoomTypeId",
                schema: "ModuleHotel",
                table: "Amenities",
                column: "RoomTypeId",
                principalSchema: "ModuleHotel",
                principalTable: "RoomTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}