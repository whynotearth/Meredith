using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class SeperateProductTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pages_Prices_PriceId",
                table: "Pages");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderLines_Pages_ProductId",
                schema: "ModuleShop",
                table: "OrderLines");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductLocationInventories_Pages_ProductId",
                schema: "ModuleShop",
                table: "ProductLocationInventories");

            migrationBuilder.DropForeignKey(
                name: "FK_Variations_Pages_ProductId",
                schema: "ModuleShop",
                table: "Variations");

            migrationBuilder.DropIndex(
                name: "IX_Pages_PriceId",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "PriceId",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Pages");

            migrationBuilder.CreateTable(
                name: "Products",
                schema: "ModuleShop",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    PageId = table.Column<int>(nullable: false),
                    PriceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Pages_PageId",
                        column: x => x.PageId,
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_Prices_PriceId",
                        column: x => x.PriceId,
                        principalSchema: "ModuleShop",
                        principalTable: "Prices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_PageId",
                schema: "ModuleShop",
                table: "Products",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_PriceId",
                schema: "ModuleShop",
                table: "Products",
                column: "PriceId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLines_Products_ProductId",
                schema: "ModuleShop",
                table: "OrderLines",
                column: "ProductId",
                principalSchema: "ModuleShop",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLocationInventories_Products_ProductId",
                schema: "ModuleShop",
                table: "ProductLocationInventories",
                column: "ProductId",
                principalSchema: "ModuleShop",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Variations_Products_ProductId",
                schema: "ModuleShop",
                table: "Variations",
                column: "ProductId",
                principalSchema: "ModuleShop",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderLines_Products_ProductId",
                schema: "ModuleShop",
                table: "OrderLines");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductLocationInventories_Products_ProductId",
                schema: "ModuleShop",
                table: "ProductLocationInventories");

            migrationBuilder.DropForeignKey(
                name: "FK_Variations_Products_ProductId",
                schema: "ModuleShop",
                table: "Variations");

            migrationBuilder.DropTable(
                name: "Products",
                schema: "ModuleShop");

            migrationBuilder.AddColumn<int>(
                name: "PriceId",
                table: "Pages",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Pages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_PriceId",
                table: "Pages",
                column: "PriceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pages_Prices_PriceId",
                table: "Pages",
                column: "PriceId",
                principalSchema: "ModuleShop",
                principalTable: "Prices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLines_Pages_ProductId",
                schema: "ModuleShop",
                table: "OrderLines",
                column: "ProductId",
                principalTable: "Pages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLocationInventories_Pages_ProductId",
                schema: "ModuleShop",
                table: "ProductLocationInventories",
                column: "ProductId",
                principalTable: "Pages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Variations_Pages_ProductId",
                schema: "ModuleShop",
                table: "Variations",
                column: "ProductId",
                principalTable: "Pages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
