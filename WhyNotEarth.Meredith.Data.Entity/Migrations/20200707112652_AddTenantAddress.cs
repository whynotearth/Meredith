using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WhyNotEarth.Meredith.Data.Entity.Migrations
{
    public partial class AddTenantAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "Tenants",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Addresses",
                schema: "ModuleShop",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Street = table.Column<string>(nullable: true),
                    ApartmentNumber = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    ZipCode = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_AddressId",
                table: "Tenants",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_Addresses_AddressId",
                table: "Tenants",
                column: "AddressId",
                principalSchema: "ModuleShop",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_Addresses_AddressId",
                table: "Tenants");

            migrationBuilder.DropTable(
                name: "Addresses",
                schema: "ModuleShop");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_AddressId",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Tenants");
        }
    }
}
