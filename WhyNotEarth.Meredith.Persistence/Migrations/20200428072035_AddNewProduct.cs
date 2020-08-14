using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class AddNewProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ModuleShop");

            migrationBuilder.RenameTable(
                name: "Reservations",
                schema: "ModuleHotel",
                newName: "Reservations",
                newSchema: "ModuleShop");

            migrationBuilder.RenameTable(
                name: "Prices",
                schema: "ModuleHotel",
                newName: "Prices",
                newSchema: "ModuleShop");

            migrationBuilder.RenameTable(
                name: "Payments",
                schema: "ModuleHotel",
                newName: "Payments",
                newSchema: "ModuleShop");

            migrationBuilder.AddColumn<int>(
                name: "PriceId",
                table: "Pages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Pages",
                nullable: false,
                defaultValue: "Page"); // Custom

            migrationBuilder.AlterColumn<DateTime>(
                name: "Start",
                schema: "ModuleShop",
                table: "Reservations",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<int>(
                name: "RoomId",
                schema: "ModuleShop",
                table: "Reservations",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "NumberOfGuests",
                schema: "ModuleShop",
                table: "Reservations",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "End",
                schema: "ModuleShop",
                table: "Reservations",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AddColumn<string>(
                name: "AddressFriendlyName",
                schema: "ModuleShop",
                table: "Reservations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Apartment",
                schema: "ModuleShop",
                table: "Reservations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                schema: "ModuleShop",
                table: "Reservations",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryDateTime",
                schema: "ModuleShop",
                table: "Reservations",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                schema: "ModuleShop",
                table: "Reservations",
                nullable: false,
                defaultValue: "HotelReservation"); // Custom

            migrationBuilder.AddColumn<string>(
                name: "Floor",
                schema: "ModuleShop",
                table: "Reservations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GeoLocation",
                schema: "ModuleShop",
                table: "Reservations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StreetAddress",
                schema: "ModuleShop",
                table: "Reservations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WhereToPark",
                schema: "ModuleShop",
                table: "Reservations",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RoomTypeId",
                schema: "ModuleShop",
                table: "Prices",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                schema: "ModuleShop",
                table: "Prices",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                schema: "ModuleShop",
                table: "Prices",
                nullable: false,
                defaultValue: "HotelPrice"); // Custom

            migrationBuilder.CreateTable(
                name: "Locations",
                schema: "ModuleShop",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                schema: "ModuleShop",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Variations",
                schema: "ModuleShop",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ProductId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Variations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Variations_Pages_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductLocationInventories",
                schema: "ModuleShop",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ProductId = table.Column<int>(nullable: false),
                    LocationId = table.Column<int>(nullable: false),
                    Count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductLocationInventories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductLocationInventories_Locations_LocationId",
                        column: x => x.LocationId,
                        principalSchema: "ModuleShop",
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductLocationInventories_Pages_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                schema: "ModuleShop",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    PaymentMethodId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalSchema: "ModuleShop",
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderLines",
                schema: "ModuleShop",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    OrderId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    Count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderLines_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "ModuleShop",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderLines_Pages_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pages_PriceId",
                table: "Pages",
                column: "PriceId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLines_OrderId",
                schema: "ModuleShop",
                table: "OrderLines",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLines_ProductId",
                schema: "ModuleShop",
                table: "OrderLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentMethodId",
                schema: "ModuleShop",
                table: "Orders",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLocationInventories_LocationId",
                schema: "ModuleShop",
                table: "ProductLocationInventories",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLocationInventories_ProductId",
                schema: "ModuleShop",
                table: "ProductLocationInventories",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Variations_ProductId",
                schema: "ModuleShop",
                table: "Variations",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pages_Prices_PriceId",
                table: "Pages",
                column: "PriceId",
                principalSchema: "ModuleShop",
                principalTable: "Prices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pages_Prices_PriceId",
                table: "Pages");

            migrationBuilder.DropTable(
                name: "OrderLines",
                schema: "ModuleShop");

            migrationBuilder.DropTable(
                name: "ProductLocationInventories",
                schema: "ModuleShop");

            migrationBuilder.DropTable(
                name: "Variations",
                schema: "ModuleShop");

            migrationBuilder.DropTable(
                name: "Orders",
                schema: "ModuleShop");

            migrationBuilder.DropTable(
                name: "Locations",
                schema: "ModuleShop");

            migrationBuilder.DropTable(
                name: "PaymentMethods",
                schema: "ModuleShop");

            migrationBuilder.DropIndex(
                name: "IX_Pages_PriceId",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "PriceId",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "AddressFriendlyName",
                schema: "ModuleShop",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Apartment",
                schema: "ModuleShop",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "City",
                schema: "ModuleShop",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "DeliveryDateTime",
                schema: "ModuleShop",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                schema: "ModuleShop",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Floor",
                schema: "ModuleShop",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "GeoLocation",
                schema: "ModuleShop",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "StreetAddress",
                schema: "ModuleShop",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "WhereToPark",
                schema: "ModuleShop",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                schema: "ModuleShop",
                table: "Prices");

            migrationBuilder.RenameTable(
                name: "Reservations",
                schema: "ModuleShop",
                newName: "Reservations",
                newSchema: "ModuleHotel");

            migrationBuilder.RenameTable(
                name: "Prices",
                schema: "ModuleShop",
                newName: "Prices",
                newSchema: "ModuleHotel");

            migrationBuilder.RenameTable(
                name: "Payments",
                schema: "ModuleShop",
                newName: "Payments",
                newSchema: "ModuleHotel");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Start",
                schema: "ModuleHotel",
                table: "Reservations",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RoomId",
                schema: "ModuleHotel",
                table: "Reservations",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NumberOfGuests",
                schema: "ModuleHotel",
                table: "Reservations",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "End",
                schema: "ModuleHotel",
                table: "Reservations",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RoomTypeId",
                schema: "ModuleHotel",
                table: "Prices",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                schema: "ModuleHotel",
                table: "Prices",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}